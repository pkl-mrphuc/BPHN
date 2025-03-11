namespace BPHN.ModelLayer.Requests
{
    public sealed class UpdateItemRequest
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double SalePrice { get; set; }
        public double PurchasePrice { get; set; }
        public string Status { get; set; }
        public string Unit { get; set; }
    }
}
