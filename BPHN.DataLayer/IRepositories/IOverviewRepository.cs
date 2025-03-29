namespace BPHN.DataLayer.IRepositories
{
    public interface IOverviewRepository
    {
        Task<object> GetTotalBookingYear(Guid accountId, DateTime now);
        Task<object> GetTotalBookingDay(Guid accountId, DateTime now);
        Task<object> GetTotalDetailBookingDay(Guid accountId, DateTime now);
        Task<object> GetRevenueDay(Guid accountId, DateTime now);
        Task<object> GetRevenueMonth(Guid accountId, DateTime now);
        Task<object> GetRevenueQuarter(Guid accountId, DateTime now);
        Task<object> GetRevenueYear(Guid accountId, DateTime now);
        Task<object> GetRevenueServiceYear(Guid accountId, DateTime now);
        Task<(int draft, int published)> GetTotalInvoice(Guid accountId);
    }
}
