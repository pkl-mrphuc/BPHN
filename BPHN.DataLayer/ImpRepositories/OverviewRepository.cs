using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class OverviewRepository : BaseRepository, IOverviewRepository
    {
        public OverviewRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<object> GetRevenueDay(Guid accountId, DateTime now)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var result = await connection.QueryFirstOrDefaultAsync<object>(Query.STATISTIC__GET_REVENUE, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@val1", now.ToString("yyyy-MM-dd 00:00:00") },
                    { "@val2", now.ToString("yyyy-MM-dd 23:59:59") },
                    { "@preVal1", now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00") },
                    { "@preVal2", now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@parameter", now },
                });
                return result;
            }
        }

        public async Task<object> GetRevenueMonth(Guid accountId, DateTime now)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var firstDayOfThisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
                var result = await connection.QueryFirstOrDefaultAsync<object>(Query.STATISTIC__GET_REVENUE, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@val1", firstDayOfThisMonth.ToString("yyyy-MM-dd 00:00:00") },
                    { "@val2", firstDayOfThisMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@preVal1", firstDayOfThisMonth.AddMonths(-1).AddDays(1).ToString("yyyy-MM-dd 00:00:00") },
                    { "@preVal2", firstDayOfThisMonth.AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@parameter", now },
                });
                return result;
            }
        }

        public async Task<object> GetRevenueQuarter(Guid accountId, DateTime now)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                int quarter = (now.Month - 1) / 3 + 1;
                var firstDayOfThisQuarter = new DateTime(now.Year, (quarter - 1) * 3 + 1, 1);
                var result = await connection.QueryFirstOrDefaultAsync<object>(Query.STATISTIC__GET_REVENUE, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@val1", firstDayOfThisQuarter.ToString("yyyy-MM-dd 00:00:00") },
                    { "@val2", firstDayOfThisQuarter.AddMonths(3).AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@preVal1", firstDayOfThisQuarter.AddMonths(-3).AddDays(1).ToString("yyyy-MM-dd 00:00:00") },
                    { "@preVal2", firstDayOfThisQuarter.AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@parameter", now },
                });
                return result;
            }
        }

        public async Task<object> GetRevenueYear(Guid accountId, DateTime now)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var firstDayOfThisYear = new DateTime(now.Year, 1, 1, 0, 0, 0);
                var result = await connection.QueryFirstOrDefaultAsync<object>(Query.STATISTIC__GET_REVENUE, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@val1", firstDayOfThisYear.ToString("yyyy-MM-dd 00:00:00") },
                    { "@val2", firstDayOfThisYear.AddYears(1).AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@preVal1", firstDayOfThisYear.AddYears(-1).AddDays(1).ToString("yyyy-MM-dd 00:00:00") },
                    { "@preVal2", firstDayOfThisYear.AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@parameter", now },
                });
                return result;
            }
        }

        public async Task<object> GetTotalBookingDay(Guid accountId, DateTime now)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var result = await connection.QueryFirstOrDefaultAsync<object>(Query.STATISTIC__GET_TOTAL_BOOKING, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@val1", now.ToString("yyyy-MM-dd 00:00:00") },
                    { "@val2", now.ToString("yyyy-MM-dd 23:59:59") },
                    { "@preVal1", now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00") },
                    { "@preVal2", now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@parameter", now },
                });
                return result;
            }
        }

        public async Task<object> GetTotalBookingYear(Guid accountId, DateTime now)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var firstDayOfThisYear = new DateTime(now.Year, 1, 1, 0, 0, 0);
                var result = await connection.QueryFirstOrDefaultAsync<object>(Query.STATISTIC__GET_TOTAL_BOOKING, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@val1", firstDayOfThisYear.ToString("yyyy-MM-dd 00:00:00") },
                    { "@val2", firstDayOfThisYear.AddYears(1).AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@preVal1", firstDayOfThisYear.AddYears(-1).AddDays(1).ToString("yyyy-MM-dd 00:00:00") },
                    { "@preVal2", firstDayOfThisYear.AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@parameter", now },
                });
                return result;
            }
        }

        public async Task<object> GetTotalDetailBookingDay(Guid accountId, DateTime now)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var result = await connection.QueryFirstOrDefaultAsync<object>(Query.STATISTIC__GET_TOTAL_DETAIL_BOOKING, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@val1", now.ToString("yyyy-MM-dd 00:00:00") },
                    { "@val2", now.ToString("yyyy-MM-dd 23:59:59") },
                    { "@parameter", now },
                });
                return result;
            }
        }

        public async Task<object> GetRevenueServiceYear(Guid accountId, DateTime now)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var firstDayOfThisYear = new DateTime(now.Year, 1, 1, 0, 0, 0);
                var result = await connection.QueryFirstOrDefaultAsync<object>(Query.STATISTIC__GET_REVENUE_SERVICE_YEAR, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@val1", firstDayOfThisYear.ToString("yyyy-MM-dd 00:00:00") },
                    { "@val2", firstDayOfThisYear.AddYears(1).AddDays(-1).ToString("yyyy-MM-dd 23:59:59") },
                    { "@parameter", now },
                });
                return result;
            }
        }

        public async Task<(int draft, int published)> GetTotalInvoice(Guid accountId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var result = await connection.QueryFirstOrDefaultAsync<(int draft, int published)>(Query.STATISTIC__GET_TOTAL_INVOICE, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@status1", InvoiceStatusEnum.DRAFT.ToString() },
                    { "@status2", InvoiceStatusEnum.PAID.ToString() },
                });
                return result;
            }
        }
    }
}
