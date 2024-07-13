using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Chen.EventBus;
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services,string queueName,
        params Assembly[] assemblies)
    {
        return AddEventBus(services,queueName, assemblies.ToList());
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName,
      IEnumerable<Assembly> assemblies)
    {
        List<Type> eventHandles = new List<Type>();
        foreach (Assembly assembly in assemblies)
        {
            // 用 GetTypes(), 这样非 public 类也能注册
            var types = assembly.GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IIntegrationEventHandler)));
            eventHandles.AddRange(types);
        }
        return AddEventBus(services,queueName, eventHandles);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="queueName">
    /// 如果多个消费者订阅同一个 Queue，这时 Queue 中的消息会被平均分摊给多个消费者进行处理，
    /// 而不是每个消费者都受到所有的消息并处理。
    /// 为了确保一个应用监听到所有的领域事件，所以不同前端项目的 queueName 需要不一样。
    /// 因此，对于同一个应用，这个 queueName 需要保证在多个集群实例和多次运行保持一致，
    /// 这样可以保证应用重启后仍然能够收到没来得及处理的消息，而且这样同一个应用的多个集群实例只有一个能收到消息。
    /// 不会同一条消息被一个应用的多个实例处理。这样消息的处理就被均摊到多个实例中。
    /// </param>
    /// <param name="eventHandleTypes"></param>
    /// <returns></returns>
    public static IServiceCollection AddEventBus(this IServiceCollection services,
        string queueName, IEnumerable<Type> eventHandleTypes)
    {
        foreach (var type in eventHandleTypes)
        {
            services.AddScoped(type, type);
        }
        services.AddSingleton<IEventBus>(sp =>
        {
            // 如果注册服务的时候就要读取配置，那么可以用 AddSingleton 的 Func<IServiceProvider, TService> 这个重载，
            // 因为可以拿到IServiceProvider，省的自己构建 IServieProvider
            var optionMQ = sp.GetRequiredService<IOptions<IntegrationEventRabbitMQOptions>>().Value;
            var factory = new ConnectionFactory()
            {
                HostName = optionMQ.HostName,
                DispatchConsumersAsync = true,
            };
            // eventBus 归 DI 管理，释放的时候会调用 Dispose
            // eventBus 的 Dispose 中会销毁 RabbitMQConnection
            RabbitMQConnection rabbitMQConnection = new(factory);
            var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            RabbitMQEventBus eventBus = new(rabbitMQConnection, serviceScopeFactory, optionMQ.ExchangeName, queueName);
            // 遍历所有实现了 IIntegrationEventHandler 接口的类，然后把他们批量注册到 eventBus
            foreach (var type in eventHandleTypes)
            {
                // 获取类上标注的EventNameAttribute，EventNameAttribute的Name为要监听的名字
                // 允许监听多个事件，但是不能为空
                var eventNameAttrs = type.GetCustomAttributes<EventNameAttribute>();
                if (!eventNameAttrs.Any())
                {
                    throw new ApplicationException($"There shoule be at least one EventNameAttribute on {type}");
                }
                foreach (var eventNameAttr in eventNameAttrs)
                {
                    eventBus.Subscribe(eventNameAttr.Name, type);
                }
            }
            return eventBus;
        });
        return services;
    }
}
