using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.EventBus
{
    /// <summary>
    /// RabbitMQ 连接类
    /// </summary>
    class RabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;
        private readonly object sync_root = new object();


        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public bool IsConnected 
        { 
            get 
            {
                return _connection != null && _connection.IsOpen && !_disposed; 
            }
        }

        // 先创建Connection,再创建信道
        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (sync_root)
            {
                _connection = _connectionFactory.CreateConnection();

                if (IsConnected)
                {
                    // 为事件添加委托，在出现下面三种情况时重新尝试 Connect
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    return true;
                }
                else
                {
                return false;
                }
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }
        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }
        void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }
    }
}
