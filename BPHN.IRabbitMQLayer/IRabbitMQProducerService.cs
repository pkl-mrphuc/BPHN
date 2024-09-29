using BPHN.ModelLayer;

namespace BPHN.IRabbitMQLayer
{
    public interface IRabbitMQProducerService
    {
        bool Publish(ObjectQueue param);
    }
}
