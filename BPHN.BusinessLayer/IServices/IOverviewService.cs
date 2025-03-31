using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Requests;

namespace BPHN.BusinessLayer.IServices
{
    public interface IOverviewService
    {
        Task<ServiceResultModel> GetStatistics(GetStatisticsRequest request);
        Task<StatisticDataModel> GetTotalInvoices(Guid accountId);
    }
}
