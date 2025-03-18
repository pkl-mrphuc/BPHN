namespace BPHN.ModelLayer
{
    public class InvoiceBookingDetail : BaseModel
    {
        public Guid BookingDetailId { get; set; }
        public Guid InvoiceId { get; set; }
    }
}
