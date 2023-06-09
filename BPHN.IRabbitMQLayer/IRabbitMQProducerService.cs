using BPHN.ModelLayer;

namespace BPHN.IRabbitMQLayer
{
    public interface IRabbitMQProducerService
    {
        void Publish(ObjectQueue param);
    }
}
