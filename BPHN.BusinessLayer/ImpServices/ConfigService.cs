using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
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
            if (context is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            List<Config>? lstConfig = null;
            if (string.IsNullOrWhiteSpace(key))
            {
                lstConfig = await GetConfigs(context.Id);
            }
            else
            {
                var config = await _configRepository.GetByKey(context.Id, key);
                if (config is not null)
                {
                    lstConfig = new List<Config> { config };
                }
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<ConfigRespond>>(lstConfig)
            };
        }

        public async Task<ServiceResultModel> Save(List<Config> configs)
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

            if (configs is null)
            {
                return new ServiceResultModel
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
            if (saveResult)
            {
                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.SAVE,
                        Entity = EntityEnum.CONFIG.ToString(),
                        Data = new HistoryLogDescription
                        {
                            ModelId = context.Id,
                            OldData = JsonConvert.SerializeObject(oldConfigs),
                            NewData = JsonConvert.SerializeObject(configs)
                        }
                    }, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = saveResult
            };
        }

        public async Task<bool> AllowMultiUser(Guid accountId)
        {
            return "true".Equals(await GetValueByKey(accountId, "MultiUser"));
        }

        public async Task<string> Language(Guid accountId)
        {
            return await GetValueByKey(accountId, "Language");
        }

        public async Task<Dictionary<string, string>> GetByKey(Guid accountId, params string[] keys)
        {
            return await _configRepository.GetByKey(accountId, keys);
        }

        public async Task<List<Config>> GetConfigs(Guid accountId)
        {
            return await _configRepository.GetConfigs(accountId);
        }

        public async Task<Config> GetByKey(Guid accountId, string key)
        {
            return await _configRepository.GetByKey(accountId, key);
        }

        public async Task<string> GetValueByKey(Guid accountId, string key)
        {
            return await _configRepository.GetValueByKey(accountId, key);
        }
    }
}
