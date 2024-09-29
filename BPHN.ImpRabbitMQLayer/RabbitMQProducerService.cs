using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace BPHN.ImpRabbitMQLayer
{
    public class RabbitMQProducerService : IRabbitMQProducerService
    {
        private readonly AppSettings _appSettings;
        private readonly IModel? _model;
        public RabbitMQProducerService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _model = CreateChannel()?.CreateModel();
        }

        private IConnection? CreateChannel()
        {
            try
            {
                var connection = new ConnectionFactory
                {
                    UserName = _appSettings.RabbitMQConfiguration.Username,
                    Password = _appSettings.RabbitMQConfiguration.Password,
                    HostName = _appSettings.RabbitMQConfiguration.HostName
                };
                connection.DispatchConsumersAsync = true;
                var channel = connection.CreateConnection();
                return channel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Publish(ObjectQueue param)
        {
            if (_model is null) return false;

            var body = Encoding.UTF8.GetBytes(param.DataJson);
            _model.ExchangeDeclare(exchange: "BPHN.Queue", type: ExchangeType.Topic, durable: true);
            _model.BasicPublish(exchange: "BPHN.Queue",
                     routingKey: param.DataType,
                     basicProperties: null,
                     body: body);

            return true;
        }
    }
}
