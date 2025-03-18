namespace BPHN.ModelLayer.Responses
{
    public sealed class GetSingleInvoiceRespond
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public CustomerTypeEnum CustomerType { get; set; }
        public PaymentTypeEnum PaymentType { get; set; }
        public double Total { get; set; }
        public Guid? BookingDetailId { get; set; }
        public IEnumerable<InvoiceItem> Items { get; set; }
    }
}
