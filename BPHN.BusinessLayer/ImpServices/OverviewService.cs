using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Requests;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace BPHN.BusinessLayer.ImpServices
{
    public class OverviewService : BaseService, IOverviewService
    {
        private readonly IOverviewRepository _overviewRepository;
        private readonly IPermissionService _permissionService;
        public OverviewService(
            IServiceProvider provider, 
            IOptions<AppSettings> appSettings, 
            IOverviewRepository overviewRepository,
            IPermissionService permissionService) : base(provider, appSettings)
        {
            _overviewRepository = overviewRepository;
            _permissionService = permissionService;
        }

        public async Task<ServiceResultModel> GetStatistics(GetStatisticsRequest request)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await _permissionService.IsValidPermission(context.Id, FunctionTypeEnum.VIEWSTATISTIC);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var tasks = request.Types.ToDictionary(item => item.Name, item => GetStatistic(context.Id, item));
            await Task.WhenAll(tasks.Values);

            var result = tasks.ToDictionary(item => item.Key, item => item.Value.Result);
            return new ServiceResultModel
            {
                Success = true,
                Data = result
            };
        }

        private async Task<object> GetStatistic(Guid accountId, StatisticTypeRequest type)
        {
            Enum.TryParse(typeof(StatisticTypeEnum), type.Name, out var name);
            switch(name)
            {
                case StatisticTypeEnum.TOTALBOOKINGYEAR:
                    return await _overviewRepository.GetTotalBookingYear(accountId, DateTime.Parse((type.Parameter ?? DateTime.Now).ToString(), styles: DateTimeStyles.RoundtripKind));
                case StatisticTypeEnum.TOTALBOOKINGDAY:
                    return await _overviewRepository.GetTotalBookingDay(accountId, DateTime.Parse((type.Parameter ?? DateTime.Now).ToString(), styles: DateTimeStyles.RoundtripKind));
                case StatisticTypeEnum.REVENUEDAY:
                    return await _overviewRepository.GetRevenueDay(accountId, DateTime.Parse((type.Parameter ?? DateTime.Now).ToString(), styles: DateTimeStyles.RoundtripKind));
                case StatisticTypeEnum.REVENUEMONTH:
                    return await _overviewRepository.GetRevenueMonth(accountId, DateTime.Parse((type.Parameter ?? DateTime.Now).ToString(), styles: DateTimeStyles.RoundtripKind));
                case StatisticTypeEnum.REVENUEYEAR:
                    return await _overviewRepository.GetRevenueYear(accountId, DateTime.Parse((type.Parameter ?? DateTime.Now).ToString(), styles: DateTimeStyles.RoundtripKind));
                case StatisticTypeEnum.REVENUEQUARTER:
                    return await _overviewRepository.GetRevenueQuarter(accountId, DateTime.Parse((type.Parameter ?? DateTime.Now).ToString(), styles: DateTimeStyles.RoundtripKind));
                case StatisticTypeEnum.TOTALDETAILBOOKINGDAY:
                    return await _overviewRepository.GetTotalDetailBookingDay(accountId, DateTime.Parse((type.Parameter ?? DateTime.Now).ToString(), styles: DateTimeStyles.RoundtripKind));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
