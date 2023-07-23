namespace BPHN.ModelLayer.Others
{
    public class BookingManager
    {
        public Guid BookingDetailId { get; set; }
        public int MatchCode { get; set; }
        public DateTime MatchDate { get; set; }
        public int Weekendays { get; set; }
        public double Deposite { get; set; }
        public Guid BookingId { get; set; }
        public string BookingDetailStatus { get; set; }

        public int BookingCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BookingStatus { get; set; }
        public Guid? TimeFrameInfoId { get; set; }
        public string TimeFrameInfoName { get; set; }
        public Guid? PitchId { get; set; }
        public string PitchName { get; set; }
        public string NameDetail { get; set; }
    }
}
