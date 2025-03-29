namespace BPHN.ModelLayer.Requests
{
    public sealed class UpdateAccountRequest
    {
        public Guid Id { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string LicenseType { get; set; }
    }
}
