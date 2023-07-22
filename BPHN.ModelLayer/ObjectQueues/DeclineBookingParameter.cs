namespace BPHN.ModelLayer.ObjectQueues
{
    public class DeclineBookingParameter : SendMailParameter
    {
        public string PhoneNumber { get; set; }
        public string Reason { get; set; }
    }
}
