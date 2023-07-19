using BPHN.ModelLayer.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.Others
{
    public class BookingRequest
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? TimeFrameInfoId { get; set; }
        public Guid? PitchId { get; set; }
        public string NameDetail { get; set; }
        public string TeamA { get; set; }
        public string Note { get; set; }
        public Guid AccountId { get; set; }
    }
}
