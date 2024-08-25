namespace BPHN.ModelLayer.Requests
{
    public sealed class TimeFrameInfoRequest
    {
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
        public long TimeBeginTick { get; set; }
        public long TimeEndTick { get; set; }
        public double Price { get; set; }
    }
}
