using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetInvoices(Guid accountId, string txtSearch, string status, int? customerType, DateTime? date, int? paymentType);
        Task<Invoice> GetById(Guid id);
        Task<bool> Insert(Invoice data);
        Task<bool> Update(Invoice data);
    }
}
