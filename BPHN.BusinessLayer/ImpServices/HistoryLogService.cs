using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class HistoryLogService : BaseService, IHistoryLogService
    {
        private readonly IHistoryLogRepository _historyLogRepository;
        public HistoryLogService(IHistoryLogRepository historyLogRepository, 
            IServiceProvider provider, 
            IOptions<AppSettings> appSettings) : base (provider, appSettings) 
        {
            _historyLogRepository = historyLogRepository;
        }
        public async Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

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

            var where = new List<WhereCondition>();
            where.Add(new WhereCondition
            {
                Column = "ActorId",
                Operator = "=",
                Value = context.Id.ToString()
            });
            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                where.Add(new WhereCondition
                {
                    Column = "ActionName",
                    Operator = "like",
                    Value = $"%{txtSearch}%"
                });
            }

            var resultCountPaging = await _historyLogRepository.GetCountPaging(pageIndex, pageSize, where);
            return new ServiceResultModel
            {
                 Success = true,
                 Data = resultCountPaging
            };
        }

        public async Task<ServiceResultModel> GetDescription(string historyLogId)
        {
            return new ServiceResultModel
            {
                Success = true,
                Data = await _historyLogRepository.GetDescription(historyLogId)
            };
        }

        public async Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

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

            var where = new List<WhereCondition>();
            where.Add(new WhereCondition
            {
                Column = "ActorId",
                Operator = "=",
                Value = context.Id.ToString()
            });
            if(!string.IsNullOrWhiteSpace(txtSearch))
            {
                where.Add(new WhereCondition
                {
                    Column = "ActionName",
                    Operator = "like",
                    Value = $"%{txtSearch}%"
                });
            }
            var resultPaging = await _historyLogRepository.GetPaging(pageIndex, pageSize, where);
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<HistoryLogRespond>>(resultPaging)
            };
        }

        public async Task<ServiceResultModel> Write(HistoryLog history, Account? context)
        {

            if(context is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            switch (history.ActionType)
            {
                case ActionEnum.LOGIN:
                    history.ActionName = ActionEnum.LOGIN.ToString();
                    break;
                case ActionEnum.REGISTERACCOUNT:
                    history.ActionName = ActionEnum.REGISTERACCOUNT.ToString();
                    break;
                case ActionEnum.SENDRESETPASSWORD:
                    history.ActionName = ActionEnum.SENDRESETPASSWORD.ToString();
                    break;
                case ActionEnum.SUBMITRESETPASSWORD:
                    history.ActionName = ActionEnum.SUBMITRESETPASSWORD.ToString();
                    break;
                case ActionEnum.SAVE:
                    history.ActionName = ActionEnum.SAVE.ToString();
                    break;
                case ActionEnum.INSERT:
                    history.ActionName = ActionEnum.INSERT.ToString();
                    break;
                case ActionEnum.UPDATE:
                    history.ActionName = ActionEnum.UPDATE.ToString();
                    break;
            }

            history.ModifiedDate = DateTime.Now;
            history.ModifiedBy = context.FullName;
            history.CreatedBy = context.FullName;
            history.CreatedDate = DateTime.Now;
            history.Id = history.Id.Equals(Guid.Empty) ? Guid.NewGuid() : history.Id;

            return new ServiceResultModel
            {
                Success = true,
                Data = await _historyLogRepository.Write(history)
            };
        }
    }
}
