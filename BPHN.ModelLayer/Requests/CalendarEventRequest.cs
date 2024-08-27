namespace BPHN.ModelLayer.Requests
{
    public sealed class CalendarEventRequest
    {
        public Guid Id { get; set; }
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public string Note { get; set; }
        public double Deposite { get; set; }
    }
}
