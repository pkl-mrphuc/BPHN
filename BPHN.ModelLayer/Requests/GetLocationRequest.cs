namespace BPHN.ModelLayer.Requests
{
    public sealed class GetLocationRequest
    {
        public string TxtSearch { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
    }
}
