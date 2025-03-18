using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetInvoices(Guid accountId, string txtSearch, string status, int? customerType, DateTime? date, int? paymentType);
        Task<Invoice> GetById(Guid id);
        Task<Invoice> GetByBooking(Guid bookingDetailId);
        Task<bool> Insert(Invoice data, InvoiceBookingDetail? _);
        Task<bool> Update(Invoice data);
    }
}
