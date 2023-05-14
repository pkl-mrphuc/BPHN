using BPHN.BusinessLayer.IServices;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
