using BPHN.IRabbitMQLayer;

namespace BPHN.MailWorker
{
    public class Worker : BackgroundService
    {
        private readonly IRabbitMQComsumerService _consumer;
        public Worker(IRabbitMQComsumerService consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subcribe();   
        }
    }
}