using BPHN.ModelLayer.Attributes;

namespace BPHN.ModelLayer
{
    public class BookingDetail : BaseModel
    {
        public int MatchCode { get; set; }
        public DateTime MatchDate { get; set; }
        public int Weekendays { get; set; }
        public double Deposite { get; set; }
        [IgnoreLog]
        public Guid BookingId { get; set; }
        public string Status { get; set; }
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public string Note { get; set; }
    }
}
