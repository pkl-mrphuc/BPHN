namespace BPHN.ModelLayer.Responses
{
    public sealed class PermissionRespond
    {
        public Guid Id { get; set; }
        public int FunctionType { get; set; }
        public bool Allow { get; set; }
    }
}
