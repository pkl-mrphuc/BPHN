namespace BPHN.ModelLayer
{
    public class AppSettings
    {
        public string MailTemplateAPI { get; set; }
        public string Secret { get; set; }
        public string Secret1 { get; set; }
        public string ClientHost { get; set; }
        public RabbitMQConfiguration RabbitMQConfiguration { get; set; }
        public MailConfiguration MailConfiguration { get; set; }
        public string RedisCacheUrl { get; set; }
        public int RedisExpireHour { get; set; }
        public string ConnectionString { get; set; }
        public string FileFolder { get; set; }
        public string FileUrl { get; set; }
        public string SignalrUrl { get; set; }
        public bool EnableCacheService { get; set; }
    }

    public class RabbitMQConfiguration
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class MailConfiguration
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
