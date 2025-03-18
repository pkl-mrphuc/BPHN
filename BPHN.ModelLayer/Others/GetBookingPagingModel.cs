namespace BPHN.ModelLayer.Others
{
    public sealed class GetBookingPagingModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string TxtSearch { get; set; }
        public string Status { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? MatchDate { get; set; }
        public string Deposit { get; set; }
        public Guid? PitchId { get; set; }
        public Guid? TimeFrameId { get; set; }
        public string NameDetail { get; set; }
        public Guid AccountId { get; set; }
    }
}
