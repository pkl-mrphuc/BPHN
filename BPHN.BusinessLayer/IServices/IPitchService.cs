using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IPitchService
    {
        Task<ServiceResultModel> Insert(Pitch pitch);
        Task<ServiceResultModel> Update(Pitch pitch);
        Task<ServiceResultModel> GetInstance(string id);
        Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasDetail = false, bool hasInactive = true);
        Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasInactive = true);
        Task<Pitch?> GetById(Guid id);
        Task<List<Pitch>> GetAll(string accountId);
    }
}
