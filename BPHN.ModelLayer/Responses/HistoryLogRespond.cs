namespace BPHN.ModelLayer.Responses
{
    public sealed class HistoryLogRespond
    {
        public Guid Id { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public string Actor { get; set; }
        public string Entity { get; set; }
        public string IPAddress { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
