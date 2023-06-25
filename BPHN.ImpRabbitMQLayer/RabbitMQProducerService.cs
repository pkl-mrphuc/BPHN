using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using System.Text;

namespace BPHN.ImpRabbitMQLayer
{
    public class RabbitMQProducerService : IRabbitMQProducerService
    {
        private readonly AppSettings _appSettings;
        public RabbitMQProducerService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void Publish(ObjectQueue param)
        {
            try
            {
                Log.Debug($"RabbitMQProducer/Publish start: {JsonConvert.SerializeObject(param)}");
                var connection = CreateChannel();
                using var model = connection.CreateModel();
                model.QueueDeclare(queue: "BPHN.MailQueue",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(param));
                model.BasicPublish(exchange: string.Empty,
                                     routingKey: "BPHN.MailQueue",
                                     basicProperties: null,
                                     body: body);
            }
            catch (Exception ex)
            {
                Log.Error($"RabbitMQProducer/Publish error: {JsonConvert.SerializeObject(ex)}");
            }
            
        }

        private IConnection CreateChannel()
        {
            ConnectionFactory connection = new ConnectionFactory()
            {
                UserName = _appSettings.RabbitMQConfiguration.Username,
                Password = _appSettings.RabbitMQConfiguration.Password,
                HostName = _appSettings.RabbitMQConfiguration.HostName
            };
            connection.DispatchConsumersAsync = true;
            var channel = connection.CreateConnection();
            return channel;
        }
    }
}
