namespace BPHN.ModelLayer.ObjectQueues
{
    public class SendMailParameter
    {
        public MailTypeEnum MailType { get; set; }
        public Type ParameterType { get; set; }
        public string ReceiverAddress { get; set; }
        public bool HasAttachmentFile { get; set; } = false;
    }
}
