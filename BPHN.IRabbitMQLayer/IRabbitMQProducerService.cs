using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.IRabbitMQLayer
{
    public interface IRabbitMQProducerService
    {
        void Publish(ObjectQueue param);
    }
}
