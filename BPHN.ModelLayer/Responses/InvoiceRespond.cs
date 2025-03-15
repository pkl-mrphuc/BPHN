namespace BPHN.ModelLayer.Responses
{
    public sealed class InvoiceRespond
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public double Total { get; set; }
        public PaymentTypeEnum PaymentType { get; set; }
        public CustomerTypeEnum CustomerType { get; set; }
    }
}
