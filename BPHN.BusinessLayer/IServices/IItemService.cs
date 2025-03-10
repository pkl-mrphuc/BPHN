using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IItemService
    {
        Task<ServiceResultModel> GetItems();
        Task<ServiceResultModel> GetInstance(string id);
        Task<ServiceResultModel> Insert(Item data);
        Task<ServiceResultModel> Update(Item data);
    }
}
