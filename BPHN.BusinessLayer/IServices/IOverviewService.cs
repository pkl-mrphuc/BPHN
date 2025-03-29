using BPHN.ModelLayer;
using BPHN.ModelLayer.Requests;

namespace BPHN.BusinessLayer.IServices
{
    public interface IOverviewService
    {
        Task<ServiceResultModel> GetStatistics(GetStatisticsRequest request);

        Task<(int draft, int published)> GetTotalInvoices(Guid accountId);
    }
}
