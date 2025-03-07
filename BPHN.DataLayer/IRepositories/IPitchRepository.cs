using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.DataLayer.IRepositories
{
    public interface IPitchRepository
    {
        Task<List<Pitch>> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where);
        Task<object> GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where);
        Task<Pitch?> GetById(Guid id);
        Task<List<Pitch>> GetAll(string accountId);
        Task<bool> Insert(Pitch pitch);
        Task<bool> Update(Pitch pitch);
    }
}
