namespace BPHN.ModelLayer.Others
{
    public class ExpireResetPasswordModel
    {
        public DateTime ExpireTime { get; set; }
        public string AccountId { get; set; }
        public string Password { get; set; }
    }
}
