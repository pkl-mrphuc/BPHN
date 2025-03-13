namespace BPHN.ModelLayer
{
    public class Invoice : BaseModel
    {
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public int CustomerType { get; set; }
        public int PaymentType { get; set; }
        public double Total { get; set; }
        public string Detail { get; set; }
    }
}
