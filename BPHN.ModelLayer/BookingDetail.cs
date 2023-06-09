namespace BPHN.ModelLayer
{
    public class BookingDetail : BaseModel
    {
        public DateTime MatchDate { get; set; }
        public double Deposite { get; set; }
        public Guid BookingId { get; set; }
        public string Status { get; set; }
    }
}
