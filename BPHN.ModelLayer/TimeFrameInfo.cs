using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer
{
    public class TimeFrameInfo : BaseModel
    {
        public int SortOrder { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public DateTime TimeBegin { get; set; }
        [Required]
        public DateTime TimeEnd { get; set; }
        public double Price { get; set; }
        [Required]
        public Guid PitchId { get; set; }
    }
}
