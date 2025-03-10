using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer
{
    public class Item : BaseModel
    {
        public Guid AccountId { get; set; }
        public string Status { get; set; }
        [Required]
        [MaxLength(36)]
        public string Code { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Range(0, 1000)]
        public int Quantity { get; set; }
        public double SalePrice { get; set; }
        public double PurchasePrice { get; set; }
        public string Unit { get; set; }
    }
}
