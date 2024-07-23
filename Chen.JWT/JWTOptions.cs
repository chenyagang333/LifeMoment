namespace Chen.JWT
{
    public class JWTOptions
    {
        /// <summary>
        /// 令牌的发行者
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 令牌的受众
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 令牌的密钥
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 令牌的受众过期时间
        /// </summary>
        public int ExpireSeconds { get; set; }

        /// <summary>
        /// 使用SignalR的 app.MapHub<MyHub>("/MyHub"); 中的参数
        /// </summary>
        public string? SignalRMapHubPattern { get; set; }
    }
}
