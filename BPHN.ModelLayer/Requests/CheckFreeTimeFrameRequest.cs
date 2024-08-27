namespace BPHN.ModelLayer.Requests
{
    public sealed class CheckFreeTimeFrameRequest
    {
        public string Email { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime EndDate { get; set; }
        public string NameDetail { get; set; }
        public string PhoneNumber { get; set; }
        public Guid PitchId { get; set; }
        public DateTime StartDate { get; set; }
        public Guid TimeFrameInfoId { get; set; }
        public int? Weekendays { get; set; }
    }
}
