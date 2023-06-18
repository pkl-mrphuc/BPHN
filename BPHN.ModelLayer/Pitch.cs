using BPHN.ModelLayer.Attributes;
using System.ComponentModel.DataAnnotations;

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
        [Range(30, 1440)]
        public int MinutesPerMatch { get; set; } = 90;
        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
        [Required]
        [Range(1, 48)]
        public int TimeSlotPerDay { get; set; } = 1;
        [IgnoreLog]
        public string TimeFrameInfoIds { get; set; }
        [IgnoreLog]
        public List<TimeFrameInfo> TimeFrameInfos { get; set; }
        public string NameDetails  { get; set; }
        [IgnoreLog]
        public List<string> ListNameDetails { get; set; }
        [IgnoreLog]
        public Guid ManagerId { get; set; }
        public string Status { get; set; } = ActiveStatusEnum.ACTIVE.ToString();
        public string AvatarUrl { get; set; }
    }
}
