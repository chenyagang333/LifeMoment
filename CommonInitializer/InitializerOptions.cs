namespace CommonInitializer
{
    public class InitializerOptions
    {
        public string LogFilePath { get; set; }

        //用于EventBus的QueueName，因此要维持“同一个项目值保持一直，不同项目不能冲突”
        public string EventBusQueueName { get; set; }
        /// <summary>
        /// AutoMapper
        /// </summary>
        public Type[] ProfileAssemblyMarkerTypes { get; set; }
        /// <summary>
        /// 数据库连接字符串的Key
        /// </summary>
        public string? ConStrKey { get; set; }
    }
}
