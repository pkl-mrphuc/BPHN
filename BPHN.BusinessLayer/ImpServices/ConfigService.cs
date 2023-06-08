﻿using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Newtonsoft.Json;
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
        private readonly IHistoryLogService _historyLogService;
        private readonly ICacheService _cacheService;
        public ConfigService(IConfigRepository configRepository,
            IContextService contextService,
            IHistoryLogService historyLogService,
            ICacheService cacheService)
        {
            _configRepository = configRepository;
            _contextService = contextService;
            _historyLogService = historyLogService;
            _cacheService = cacheService;
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

            var data = _cacheService.Get(_cacheService.GetKeyCache(key??"All", "Config"));

            if(string.IsNullOrEmpty(data))
            {
                var result = _configRepository.GetConfigs(context.Id, key);
                if(string.IsNullOrEmpty(key))
                {
                    _cacheService.Set(_cacheService.GetKeyCache("All", "Config"), JsonConvert.SerializeObject(result));
                }

                return new ServiceResultModel()
                {
                    Success = true,
                    Data = result
                };
            }
            else
            {
                var result = JsonConvert.DeserializeObject<List<Config>>(data);

                return new ServiceResultModel()
                {
                    Success = true,
                    Data = result
                };
            }

            
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
                item.AccountId = context.Id;
                item.Id = Guid.NewGuid();
                return item;
            }).ToList();

            bool saveResult = _configRepository.Save(configs);
            if(saveResult)
            {
                _cacheService.Remove(_cacheService.GetKeyCache("All", "Config"));
                Thread thread = new Thread(delegate ()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.SAVE_CONFIG,
                        ActionName = string.Empty
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
