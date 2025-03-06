namespace BPHN.ModelLayer.Responses
{
    public sealed class GetPagingPitchRespond
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string AvatarUrl { get; set; }
        public List<TimeFrameInfoRespond> TimeFrameInfos { get; set; }
        public string NameDetails { get; set; }
        public Guid ManagerId { get; set; }
    }
}
