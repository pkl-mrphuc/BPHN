using BPHN.BusinessLayer.IServices;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace BPHN.ImpRabbitMQLayer
{
    public class RabbitMQConsumerService : IRabbitMQComsumerService, IDisposable
    {
        private readonly AppSettings _appSettings;
        private readonly IModel _model;
        private readonly IConnection _connection;
        private readonly IEmailWorker _mailWorker;

        public RabbitMQConsumerService(IOptions<AppSettings> appSettings, IServiceProvider provider)
        {
            _appSettings = appSettings.Value;
            _connection = CreateChannel();
            _model = _connection.CreateModel();
            _model.QueueDeclare(queue: "BPHN.MailQueue", 
                                durable: false, 
                                exclusive: false, 
                                autoDelete: false);

            _mailWorker = provider.GetRequiredService<IEmailWorker>();
        }

        public async Task Subcribe()
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var text = Encoding.UTF8.GetString(body);
                    Log.Debug($"RabbitMQConsumer/Subcribe start: {text}");
                    ObjectQueue parameter = JsonConvert.DeserializeObject<ObjectQueue>(text);
                    switch (parameter.QueueJobType)
                    {
                        case QueueJobTypeEnum.SENDMAIL:
                            await _mailWorker.Handle(parameter.DataJson);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"RabbitMQConsumer/Subcribe error: {JsonConvert.SerializeObject(ex)}");
                }
                
            };
            _model.BasicConsume(queue: "BPHN.MailQueue", 
                                autoAck: true, 
                                consumer: consumer);
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

        public void Dispose()
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
