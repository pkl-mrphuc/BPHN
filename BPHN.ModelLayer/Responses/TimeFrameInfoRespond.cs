namespace BPHN.ModelLayer.Responses
{
    public sealed class TimeFrameInfoRespond
    {
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
        public DateTime TimeBegin { get; set; }
        public DateTime TimeEnd { get; set; }
        public long TimeBeginTick { get; set; }
        public long TimeEndTick { get; set; }
        public double Price { get; set; }
    }
}
