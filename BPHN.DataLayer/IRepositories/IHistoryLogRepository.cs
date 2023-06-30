using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.DataLayer.IRepositories
{
    public interface IHistoryLogRepository
    {
        Task<bool> Write(HistoryLog history);
        Task<HistoryLogDescription?> GetDescription(string historyLogId);
        Task<List<HistoryLog>> GetPaging(int pageIndex, int pageSize, List<WhereCondition> wheres);
        Task<object> GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> wheres);
    }
}
