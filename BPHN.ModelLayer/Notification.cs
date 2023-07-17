namespace BPHN.ModelLayer
{
    public class Notification : BaseModel
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public int NotificationType { get; set; }
        public Guid AccountId { get; set; }
    }
}
