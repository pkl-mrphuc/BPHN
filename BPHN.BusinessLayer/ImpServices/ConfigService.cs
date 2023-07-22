using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class ConfigService : BaseService, IConfigService
    {
        private readonly IConfigRepository _configRepository;
        private readonly IHistoryLogService _historyLogService;
        public ConfigService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IConfigRepository configRepository,
            IHistoryLogService historyLogService) : base(serviceProvider, appSettings)
        {
            _configRepository = configRepository;
            _historyLogService = historyLogService;
        }

        public async Task<ServiceResultModel> GetConfigs(string? key = null)
        {
            var context = _contextService.GetContext();
            if (context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }

            var cacheResult = await _cacheService.GetAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.CONFIG, key ?? ""));

            if(string.IsNullOrWhiteSpace(cacheResult))
            {
                var result = await _configRepository.GetConfigs(context.Id, key);
                if(string.IsNullOrWhiteSpace(key) && result != null && result.Count > 0)
                {
                    await _cacheService.SetAsync(_cacheService.GetKeyCache(context.Id, EntityEnum.CONFIG), JsonConvert.SerializeObject(result));
                }

                return new ServiceResultModel()
                {
                    Success = true,
                    Data = result
                };
            }
            else
            {
                var result = JsonConvert.DeserializeObject<List<Config>>(cacheResult);

                return new ServiceResultModel()
                {
                    Success = true,
                    Data = result
                };
            }
        }

        public async Task<ServiceResultModel> Save(List<Config> configs)
        {
            var context = _contextService.GetContext();
            if (context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }

            if (configs == null)
            {
                return new ServiceResultModel()
                {
                    Success = true,
                    Data = true
                };
            }

            configs = configs.Select(item =>
            {
                item.ModifiedDate = DateTime.Now;
                item.ModifiedBy = context.FullName;
                item.CreatedBy = context.FullName;
                item.CreatedDate = DateTime.Now;
                item.AccountId = context.Id;
                item.Id = Guid.NewGuid();
                return item;
            }).ToList();

            var oldConfigs = await _configRepository.GetConfigs(context.Id);
            var saveResult = await _configRepository.Save(configs);
            if(saveResult)
            {
                var key = _cacheService.GetKeyCache(context.Id, EntityEnum.CONFIG);
                await _cacheService.RemoveAsync(key);
                var thread = new Thread(delegate ()
                {
                    var historyLogId = Guid.NewGuid();
                    _historyLogService.Write(new HistoryLog()
                    {
                        Id = historyLogId,
                        IPAddress = context.IPAddress,
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.SAVE,
                        Entity = "Cấu hình",
                        ActionName = string.Empty,
                        Description = BuildLinkDescription(historyLogId),
                        Data = new HistoryLogDescription()
                        {
                            ModelId = context.Id,
                            OldData = JsonConvert.SerializeObject(oldConfigs),
                            NewData = JsonConvert.SerializeObject(configs)
                        }
                    }, context);

                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = saveResult
            };
        }
    }
}
