using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public RabbitMQConfiguration RabbitMQConfiguration { get; set; }
        public MailConfiguration MailConfiguration { get; set; }
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
