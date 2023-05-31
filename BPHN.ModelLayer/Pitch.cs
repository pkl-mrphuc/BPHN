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
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)] 
        public string Address { get; set; }
        [Required]
        [Range(60, 1440)]
        public int TimeSlot { get; set; }
        public string TimeFrameInfoIds { get; set; }
        public List<TimeFrameInfo> TimeFrameInfos { get; set; }
        public Guid ManagerId { get; set; }
        public ActiveStatusEnum Status { get; set; }
        public string AvartarUrl { get; set; }
    }
}
