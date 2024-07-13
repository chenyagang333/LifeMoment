using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chen.EventBus;
class RabbitMQEventBus : IEventBus, IDisposable
{
    private IModel _consumerChannel;
    private readonly string _exchangeName;
    private string _queueName;
    private readonly RabbitMQConnection _persistentConnection;
    private readonly SubscriptionsManager _subscriptionsManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScope _serviceScope;

    public RabbitMQEventBus(RabbitMQConnection persistentConnection,
        IServiceScopeFactory serviceScopeFactory, string exchangeName, string queueName)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _subscriptionsManager = new SubscriptionsManager();
        _exchangeName = exchangeName;
        _queueName = queueName;

        // RabbitMQEventBus 是 Singleton 对象，而它创建的 IIntegrationEventHandler
        // 以及用到的 IIntegrationEventHandler 用到的服务大部分是 Scoped，因此必须显示创建一个 Scope，
        // 否则在 GetService 的时候会报错：Cannot resolvefrom root provider because it requires scoped service
        _serviceScope = serviceScopeFactory.CreateScope();
        _serviceProvider = _serviceScope.ServiceProvider;
        _consumerChannel = CreateConsumerChannel();

        _subscriptionsManager.OnEventRemoved += SubscriptionsManager_OnEventRemoved;
    }

    /// <summary>
    /// 移除订阅事件的委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventName"></param>
    private void SubscriptionsManager_OnEventRemoved(object? sender, string eventName)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        using var channel = _persistentConnection.CreateModel();

        channel.QueueUnbind(queue: _queueName, exchange: _exchangeName, routingKey: eventName);
        if (_subscriptionsManager.IsEmpty)
        {
            _queueName = string.Empty;
            _consumerChannel.Close();
        }
    }

    /// <summary>
    /// 发布消息到 RabbitMQ
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventData"></param>
    public void Publish(string eventName, object? eventData)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }
        // Channel 是建立在 Connection 上的虚拟连接
        // 创建和销毁 TCP 连接的代价非常高，
        // Connection 可以创建多个 Channel ，Channel 不是线程安全的，所以不能在线程间共享
        using var channel = _persistentConnection.CreateModel();
        // 声明交换机
        channel.ExchangeDeclare(exchange: _exchangeName, type: "direct");

        byte[] body;
        if (eventData == null)
        {
            body = new byte[0];
        }
        else
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            body = JsonSerializer.SerializeToUtf8Bytes(eventData, eventData.GetType(), options);
        }
        var properties = channel.CreateBasicProperties(); // 创建属性，对于消息做配置
        properties.DeliveryMode = 2; // persistent

        // 发布消息到 RabbitMQ
        channel.BasicPublish(
            exchange: _exchangeName, // 消息发送到的交换机名称
            routingKey: eventName,   // 消息的路由键，用于确定消息如何路由到相应的队列
            mandatory: true,         // 当无法路由到相应的队列时，将消息返回给生产者
            basicProperties: properties, // 消息的基本属性，如消息持久化、过期时间等
            body: body               // 消息体内容
        );
    }

    /// <summary>
    /// 消息订阅
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handleType"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Subscribe(string eventName, Type handleType)
    {
        CheckHandlerType(handleType); // 检查类型
        DoInternalSubscription(eventName); // 绑定队列
        _subscriptionsManager.AddSubscription(eventName, handleType); // 添加订阅
        StartBasicConsume(); // 开始消费
    }

    // 订阅事件（绑定队列）
    private void DoInternalSubscription(string eventName)
    {
        var containsKey = _subscriptionsManager.HasSubscriptionsForEvent(eventName);
        if (!containsKey) // 是否已经订阅（未订阅则绑定队列[订阅]）
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect(); // 是否已连接（未连接尝试连接）
            }
            _consumerChannel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: eventName);
        }
    }

    // 检查事件类型
    private void CheckHandlerType(Type handlerType)
    {
        if (!typeof(IIntegrationEventHandler).IsAssignableFrom(handlerType))
        {
            throw new ArgumentException($"{handlerType} doesn't inherit from IIntegrationEventHandler", nameof(handlerType));
        }
    }

    /// <summary>
    /// 创建消费者信道
    /// </summary>
    /// <returns></returns>
    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var channel = _persistentConnection.CreateModel();

        // 声明交换机
        channel.ExchangeDeclare(exchange: _exchangeName, type: "direct");

        // 声明队列
        channel.QueueDeclare(
            queue: _queueName,   // 队列名
            durable: true,       // 队列持久化，消息将保存在磁盘上以防止丢失
            exclusive: false,    // 非独占队列，允许多个连接共享同一个队列
            autoDelete: false,   // 非自动删除队列，在连接关闭后队列不会自动删除
            arguments: null      // 额外参数，例如用于设置队列的各种属性
        );

        channel.CallbackException += (sender, ea) =>
        {
            /*
            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();
            StartBasicConsume();*/
            Debug.Fail(ea.ToString());
        };

        return channel;
    }

    public void Dispose()
    {
        if (_consumerChannel != null)
        {
            _consumerChannel.Dispose();
        }
        _subscriptionsManager.Clear();
        _persistentConnection.Dispose();
        _serviceScope.Dispose();
    }


    // 移除订阅
    public void UnSubscribe(string eventName, Type handleType)
    {
        CheckHandlerType(handleType);
        _subscriptionsManager.RemoveSubscription(eventName, handleType);
    }

    public void StartBasicConsume()
    {
        if (_consumerChannel != null)
        {
            // 创建异步消费者对象 
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
            // 注册接收消息的事件处理方法
            consumer.Received += Consumer_Received;
            // 开始消费消息
            _consumerChannel.BasicConsume(queue:_queueName,autoAck:false,consumer:consumer);
        }
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey; // 这个框架中，就是用 eventName 当 RoutiongKey
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span); // 框架要求所有的消息都是字符串的json
        try
        {
            await ProcessEvent(eventName, message);
            // 如果在获取消息时采用不自动应答，但是获取消息后不调用 basicAck，
            // RabbitMQ 会认为消息没有投递成功，不仅所有的消息都会保留在内存中，
            // 而且在客户重新连接后，会将信息重新投递一次。这种情况无法完全避免，因此 EventHandler 的代码需要幂等
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            // DeliveryTag就是消息的编号 // BasicAck消息处理完成
            //multiple：批量确认标志。如果值为true，则执行批量确认，
            //此deliveryTag之前收到的消息全部进行确认; 如果值为false，则只对当前收到的消息进行确认
        }
        catch (Exception ex)
        {
            // requeue：表示如何处理这条消息，如果值为true，则重新放入 RabbitMQ 的发送队列，
            // 如果值为 false，则通知 RabbitMQ 销毁这条消息， 
            _consumerChannel.BasicReject(eventArgs.DeliveryTag, true); // BasicReject 表示消息处理失败 重新放入 RabbitMQ 的发送队列，
            //_consumerChannel.BasicReject(eventArgs.DeliveryTag, false); // BasicReject 表示消息处理失败 则通知 RabbitMQ 销毁这条消息，
            Debug.Fail(ex.ToString());
        }
    }

    /// <summary>
    /// 流程事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    private async Task ProcessEvent(string eventName, string message)
    {
        if (_subscriptionsManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptions = _subscriptionsManager.GetHandlersForEvent(eventName);
            foreach (var subscription in subscriptions)
            {
                // 各自在不同的 Scope 中，避免 DbContext 等的共享造成如下问题：
                // The instance of entity type cannot be tracked because another instance
                using var scope = _serviceProvider.CreateScope();
                IIntegrationEventHandler? handler = scope.ServiceProvider.GetService(subscription) as IIntegrationEventHandler;
                if (handler == null)
                {
                    throw new ApplicationException($"无法创建{subscription}类型的服务");
                }
                await handler.Handle(eventName, message);
            }
        }
        else
        {
            string entryAssembly = Assembly.GetEntryAssembly().GetName().Name;
            Debug.WriteLine($"找不到可以处理 eventName = {eventName} 的处理程序，entryAssembly：{entryAssembly}");
        }
    }
}
