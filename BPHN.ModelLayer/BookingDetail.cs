namespace BPHN.ModelLayer
{
    public class BookingDetail : BaseModel
    {
        public DateTime MatchDate { get; set; }
        public double Deposit { get; set; }
        public Guid BookingId { get; set; }
        public string Status { get; set; }
    }
}
