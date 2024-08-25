namespace BPHN.ModelLayer.Requests
{
    public sealed class UpdatePitchRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int MinutesPerMatch { get; set; }
        public int Quantity { get; set; }
        public int TimeSlotPerDay { get; set; }
        public List<TimeFrameInfoRequest> TimeFrameInfos { get; set; }
        public List<string> ListNameDetails { get; set; }
        public string Status { get; set; }
    }
}
