using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class Pitch
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int TimeSlot { get; set; }
        public List<TimeFrameInfo> TimeFrameInfos { get; set; }
        public Account Manager { get; set; }
    }
}
