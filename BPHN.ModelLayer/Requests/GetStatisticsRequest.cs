namespace BPHN.ModelLayer.Requests
{
    public sealed class GetStatisticsRequest
    {
        public IEnumerable<StatisticTypeRequest> Types { get; set; }
    }

    public sealed class StatisticTypeRequest
    {
        public string Name { get; set; }
        public object Parameter { get; set; }
    }
}
