using BPHN.ModelLayer.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer
{
    public class BaseModel
    {
        [Required]
        [IgnoreLog]
        public Guid Id { get; set; }
        [IgnoreLog]
        public string CreatedBy { get; set; }
        [IgnoreLog]
        public string ModifiedBy { get; set; }
        [IgnoreLog]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [IgnoreLog]
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
    }
}
