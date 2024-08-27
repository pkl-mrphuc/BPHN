namespace BPHN.ModelLayer.Requests
{
    public sealed class FindBlankRequeset
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRecurring { get; set; }
    }
}
