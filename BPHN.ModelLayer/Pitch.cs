using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class Pitch : BaseModel
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)] 
        public string Address { get; set; }
        [Required]
        [Range(60, 1440)]
        public int TimeSlot { get; set; }
        public string TimeFrameInfoIds { get; set; }
        public List<TimeFrameInfo> TimeFrameInfos { get; set; }
        public string ManagerId { get; set; }
        public Account Manager { get; set; }
    }
}
