namespace Chen.EventBus
{
    public class IntegrationEventRabbitMQOptions
    {
        /// <summary>
        /// RabbitMQ服务器地址
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName { get; set; }
    }
}
