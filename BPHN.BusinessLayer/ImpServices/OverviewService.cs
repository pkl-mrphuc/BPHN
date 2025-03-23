using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Requests;
using Microsoft.Extensions.Options;

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

            var result = new Dictionary<string, object>();
            foreach (var item in request.Types)
            {
                result.Add(item.Name, await GetStatistic(item));
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = result
            };
        }

        private async Task<object> GetStatistic(StatisticTypeRequest type)
        {
            Enum.TryParse(typeof(StatisticTypeEnum), type.Name, out var name);
            switch(name)
            {
                case StatisticTypeEnum.TOTALBOOKINGYEAR:
                    return new
                    {
                        value = 100,
                        preValue = 20,
                        parameter = type.Parameter

                    };
                case StatisticTypeEnum.TOTALBOOKINGDAY:
                    return new
                    {
                        value = 100,
                        preValue = 20,
                        parameter = type.Parameter
                    };
                case StatisticTypeEnum.REVENUEDAY:
                    return new
                    {
                        value = 100,
                        preValue = 20,
                        parameter = type.Parameter

                    };
                case StatisticTypeEnum.REVENUEMONTH:
                    return new
                    {
                        value = 100,
                        preValue = 20,
                        parameter = type.Parameter

                    };
                case StatisticTypeEnum.REVENUEYEAR:
                    return new
                    {
                        value = 100,
                        preValue = 20,
                        parameter = type.Parameter

                    };
                case StatisticTypeEnum.REVENUEQUARTER:
                    return new
                    {
                        value = 100,
                        preValue = 20,
                        parameter = type.Parameter
                    };
                default:
                    return null;
            }
        }
    }
}
