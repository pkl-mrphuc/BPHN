using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.Responses
{
    public sealed class GetSingleBookingRespond
    {
        public Guid Id { get; set; }
        public Guid? PitchId { get; set; }
        public Guid? TimeFrameInfoId { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime BookingDate { get; set; }
        public int Weekendays { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NameDetail { get; set; }
    }
}
