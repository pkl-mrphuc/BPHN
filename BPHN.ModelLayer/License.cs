namespace BPHN.ModelLayer
{
    public class License : BaseModel
    {
        public Guid AccountId { get; set; }
        public LicenseTypeEnum Type { get; set; }
        public int MaxInvoices { get; set; }
        public int MaxDraftInvoices { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
