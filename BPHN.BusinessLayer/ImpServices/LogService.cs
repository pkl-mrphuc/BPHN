using BPHN.BusinessLayer.IServices;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.ImpServices
{
    public class LogService : ILogService
    {
        private readonly IRabbitMQProducerService _producer;
        public LogService(IRabbitMQProducerService producer)
        {
            _producer = producer;
        }

        public void LogDebug(string message)
        {
            _producer.Publish(new ObjectQueue
            {
                QueueJobType = QueueJobTypeEnum.WRITELOG,
                DataType = "bphn.log.debug",
                DataJson = message
            });
        }

        public void LogError(string message)
        {
            _producer.Publish(new ObjectQueue
            {
                QueueJobType = QueueJobTypeEnum.WRITELOG,
                DataType = "bphn.log.error",
                DataJson = message
            });
        }

        public void LogInformation(string message)
        {
            _producer.Publish(new ObjectQueue
            {
                QueueJobType = QueueJobTypeEnum.WRITELOG,
                DataType = "bphn.log.information",
                DataJson = message
            });
        }

        public void LogWarning(string message)
        {
            _producer.Publish(new ObjectQueue
            {
                QueueJobType = QueueJobTypeEnum.WRITELOG,
                DataType = "bphn.log.warning",
                DataJson = message
            });
        }
    }
}
