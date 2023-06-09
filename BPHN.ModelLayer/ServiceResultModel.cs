namespace BPHN.ModelLayer
{
    public class ServiceResultModel
    {
        public bool Success { get; set; }
        public int? ErrorCode { get; set; } = null;
        public object? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
