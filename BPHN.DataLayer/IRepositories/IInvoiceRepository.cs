using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetInvoices(Guid accountId);
        Task<Invoice> GetById(Guid id);
    }
}
