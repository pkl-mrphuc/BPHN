using BPHN.BusinessLayer.IServices;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.ImpServices
{
    public class EmailService : IEmailService
    {
        private readonly IRabbitMQProducerService _producer;

        public EmailService(IRabbitMQProducerService producer)
        {
            _producer = producer;
        }

        public bool SendMail(ObjectQueue objQueue)
        {
            _producer.Publish(objQueue);
            return true;
        }
    }
}
