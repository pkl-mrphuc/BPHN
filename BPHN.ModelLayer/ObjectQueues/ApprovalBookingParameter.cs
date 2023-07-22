namespace BPHN.ModelLayer.ObjectQueues
{
    public class ApprovalBookingParameter : SendMailParameter
    {
        public string PhoneNumber { get; set; }
        public string StadiumName { get; set; }
        public string NameDetail { get; set; }
        public string BookingDate { get; set; }
        public string MatchDate { get; set; }
        public string TimeFrameInfo { get; set; }
        public string Price { get; set; }
    }
}
