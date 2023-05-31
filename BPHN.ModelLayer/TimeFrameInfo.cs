using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class TimeFrameInfo : BaseModel
    {
        public DateTime TimeBegin { get; set; }
        public DateTime TimeEnd { get; set; }
        public double Price { get; set; }
    }
}
