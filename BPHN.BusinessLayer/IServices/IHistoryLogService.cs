using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IHistoryLogService
    {
        Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch);
        Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch);
        ServiceResultModel Write(Guid id, HistoryLog history, Account? context);
        Task<ServiceResultModel> GetDescription(string historyLogId);
    }
}
