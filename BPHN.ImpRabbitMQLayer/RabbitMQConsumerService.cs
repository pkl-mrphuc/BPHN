using BPHN.BusinessLayer.IServices;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BPHN.ImpRabbitMQLayer
{
    public class RabbitMQConsumerService : IRabbitMQComsumerService, IDisposable
    {
        private readonly AppSettings _appSettings;
        private IModel? _model;
        private readonly IConnection _connection;
        private readonly IEmailWorker _mailWorker;
        private readonly ILogWorker _logWorker;

        public RabbitMQConsumerService(IOptions<AppSettings> appSettings, IServiceProvider provider)
        {
            _appSettings = appSettings.Value;
            _mailWorker = provider.GetRequiredService<IEmailWorker>();
            _logWorker = provider.GetRequiredService<ILogWorker>();
        }

        public async Task Subscribe()
        {
            _model = CreateChannel()?.CreateModel();
            if (_model is not null)
            {
                var queueName = _model.QueueDeclare().QueueName;
                _model.ExchangeDeclare(exchange: "BPHN.Queue", type: ExchangeType.Topic, durable: true);
                _model.QueueBind(queueName, "BPHN.Queue", "bphn.*.*", null);

                var consumer = new AsyncEventingBasicConsumer(_model);
                consumer.Received += async (ch, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routerKey = ea.RoutingKey;

                    switch (routerKey)
                    {
                        case "bphn.email.set-password":
                        case "bphn.email.forgot-password":
                        case "bphn.email.decline-booking":
                        case "bphn.email.approval-booking":
                            await _mailWorker.Handle(message);
                            break;
                        case "bphn.log.history":
                            await _logWorker.Handle(message);
                            break;
                    }
                };
                _model.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: consumer);
            }

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

        public void Dispose()
        {
            if (_model is not null)
            {
                if (_model.IsOpen)
                {
                    _model.Close();
                }

                if (_connection.IsOpen)
                {
                    _connection.Close();
                }
            }
        }
    }
}
