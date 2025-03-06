using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer.Responses
{
    public sealed class BookingRespond
    {
        public string BookingStatus { get; set; }
        public DateTime BookingDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BookingDetailStatus { get; set; }
        public string PitchName { get; set; }
        public string TimeFrameInfoName { get; set; }
        public string NameDetail { get; set; }
        public int Weekendays { get; set; }
        public DateTime MatchDate { get; set; }
        public double Deposite { get; set; }
        public Guid BookingDetailId { get; set; }
        public Guid BookingId { get; set; }
        public string MatchCode { get; set; }
    }
}
