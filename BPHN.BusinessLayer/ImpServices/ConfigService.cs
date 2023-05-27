using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class ConfigService : IConfigService
    {
        private readonly IConfigRepository _configRepository;
        private readonly IContextService _contextService;
        public ConfigService(IConfigRepository configRepository,
            IContextService contextService)
        {
            _configRepository = configRepository;
            _contextService = contextService;
        }

        public ServiceResultModel GetConfigs(string key = null)
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

            return new ServiceResultModel()
            {
                Success = true,
                Data = _configRepository.GetConfigs(context.Id, key)
            };
        }

        public ServiceResultModel Save(List<Config> configs)
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

            if (configs == null || (configs != null && configs.Count == 0))
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
                item.AccountId = context.Id.ToString();
                item.Id = Guid.NewGuid();
                return item;
            }).ToList();

            return new ServiceResultModel()
            {
                Success = true,
                Data = _configRepository.Save(configs)
            };
        }
    }
}
