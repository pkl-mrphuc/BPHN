using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class ObjectQueue
    {
        public QueueJobTypeEnum QueueJobType { get; set; }
        public string DataJson { get; set; }
        public Type DataType { get; set; }
    }
}
