using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IItemService
    {
        Task<ServiceResultModel> GetItems(string txtSearch, string status, string code, string unit, string quantity);
        Task<ServiceResultModel> GetInstance(string id);
        Task<ServiceResultModel> Insert(Item data);
        Task<ServiceResultModel> Update(Item data);
    }
}
