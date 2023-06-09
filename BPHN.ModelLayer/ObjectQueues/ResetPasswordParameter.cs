namespace BPHN.ModelLayer.ObjectQueues
{
    public class ResetPasswordParameter : SendMailParameter
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }     
    }
}
