namespace BPHN.ModelLayer.ViewModels
{
    public class MailSetPasswordVm
    {
        public Guid AccountId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Key { get; set; }
        public string Link { get; set; }
    }
}
