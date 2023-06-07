using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class Booking : BaseModel
    {
        [Required]
        [MaxLength(255)]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool IsRecurring { get; set; } = false;
        public DateTime BookingDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Range(0, 6)]
        public int? Weekendays { get; set; }
        public string Status { get; set; }
        [Required]
        public Guid? TimeFrameInfoId { get; set; }
        public TimeFrameInfo? TimeFrameInfo { get; set; }
        [Required]
        public Guid? PitchId { get; set; }
        public Pitch? Pitch { get; set; }
        [Required]
        [MaxLength(255)]
        public string NameDetail { get; set; }
        public string BookingDetailIds { get; set; }
        public List<BookingDetail> BookingDetails { get; set; }
    }
}
