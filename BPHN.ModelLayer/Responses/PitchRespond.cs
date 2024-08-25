namespace BPHN.ModelLayer.Responses
{
    public sealed class PitchRespond
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int MinutesPerMatch { get; set; }
        public int Quantity { get; set; }
        public int TimeSlotPerDay { get; set; }
        public List<TimeFrameInfoRespond> TimeFrameInfos { get; set; }
        public List<string> ListNameDetails { get; set; }
        public string Status { get; set; }
    }
}
