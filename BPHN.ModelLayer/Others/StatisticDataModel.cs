namespace BPHN.ModelLayer.Others
{
    public class StatisticDataModel
    {
        public int Draft { get; set; }
        public int Published { get; set; }
        public int MaxDraft { get; set; }
        public int MaxPublished { get; set; }
        public DateTime? ExpireTime { get; set; }
    }
}
