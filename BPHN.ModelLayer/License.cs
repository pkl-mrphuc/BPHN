namespace BPHN.ModelLayer
{
    public class License
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public LicenseTypeEnum Type { get; set; }
        public int MaxInvoices { get; set; }
    }
}
