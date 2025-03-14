using BPHN.ModelLayer.Attributes;

namespace BPHN.ModelLayer
{
    public class Invoice : BaseModel
    {
        public Guid AccountId { get; set; }
        public string Status { get; set; } = InvoiceStatusEnum.DRAFT.ToString();
        public DateTime Date { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public CustomerTypeEnum CustomerType { get; set; } = CustomerTypeEnum.RETAIL;
        public PaymentTypeEnum PaymentType { get; set; } = PaymentTypeEnum.BANK;
        public double Total { get; set; }
        public string Detail { get; set; }
        [IgnoreLog]
        public List<InvoiceItem>? Items { get; set; }
    }

    public class InvoiceItem
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public double SalePrice { get; set; }
        public double Total { get; set; }
    }
}
