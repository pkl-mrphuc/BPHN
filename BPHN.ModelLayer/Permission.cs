namespace BPHN.ModelLayer
{
    public class Permission : BaseModel
    {
        public Guid AccountId { get; set; }
        public int FunctionType { get; set; }
        public bool Allow { get; set; } = false;
    }
}
