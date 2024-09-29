using BPHN.BusinessLayer.IServices;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class EmailService : IEmailService
    {
        private readonly IRabbitMQProducerService _producer;

        public EmailService(IRabbitMQProducerService producer)
        {
            _producer = producer;
        }

        public bool SendMail<T>(string type, T data)
        {
            _producer.Publish(new ObjectQueue
            {
                QueueJobType = QueueJobTypeEnum.SENDMAIL,
                DataJson = JsonConvert.SerializeObject(data),
                DataType = type
            });
            return true;
        }
    }
}
