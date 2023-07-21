namespace BPHN.ModelLayer.Others
{
    public class ExpireSetPasswordModel
    {
        public DateTime ExpireTime { get; set; }
        public string AccountId { get; set; }
        public string Password { get; set; }
    }
}
