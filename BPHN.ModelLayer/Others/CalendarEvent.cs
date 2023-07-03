using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.Others
{
    public class CalendarEvent : BookingDetail
    {
        public Guid PitchId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Stadium { get; set; }
        public string PhoneNumber { get; set; }
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public string Note { get; set; }
    }
}
