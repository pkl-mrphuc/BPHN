using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IInvoiceService
    {
        Task<ServiceResultModel> GetInvoices(string txtSearch, string status, int? customerType, DateTime? date, int? paymentType);
        Task<ServiceResultModel> GetInstance(string id);
        Task<ServiceResultModel> Insert(Invoice data);
        Task<ServiceResultModel> Update(Invoice data);
    }
}
