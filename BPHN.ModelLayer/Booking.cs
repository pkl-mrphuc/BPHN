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
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool IsRecurring { get; set; } = false;
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Range(2, 8)]
        public int? Weekendays { get; set; }
        [Required]
        public string Status { get; set; }
        public Guid TimeFrameInfoId { get; set; }
        public TimeFrameInfo TimeFrameInfo { get; set; }
        public Guid PitchId { get; set; }
        public Pitch Pitch { get; set; }
    }
}
