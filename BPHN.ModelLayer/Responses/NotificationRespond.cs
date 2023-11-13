namespace BPHN.ModelLayer.Responses
{
    public class NotificationRespond
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public int NotificationType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
