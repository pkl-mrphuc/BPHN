namespace BPHN.ModelLayer.Requests
{
    public sealed class InsertInvoiceRequest
    {
        public string Status { get; set; }
        public int CustomerType { get; set; }
        public int PaymentType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public double Total { get; set; }
        public List<InvoiceItem> Items { get; set; }
    }
}
