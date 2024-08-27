namespace BPHN.ModelLayer.Responses
{
    public sealed class CalendarEventRespond
    {
        public Guid Id { get; set; }
        public Guid PitchId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid BookingId { get; set; }
        public string Stadium { get; set; }
        public string PhoneNumber { get; set; }
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public string Note { get; set; }
        public double Deposite { get; set; }
    }
}
