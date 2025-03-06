using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class HistoryLogService : BaseService, IHistoryLogService
    {
        private readonly IHistoryLogRepository _historyLogRepository;
        private readonly IRabbitMQProducerService _producer;
        public HistoryLogService(IHistoryLogRepository historyLogRepository,
            IRabbitMQProducerService producer,
            IServiceProvider provider,
            IOptions<AppSettings> appSettings) : base(provider, appSettings)
        {
            _historyLogRepository = historyLogRepository;
            _producer = producer;
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
            if (!string.IsNullOrWhiteSpace(txtSearch))
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

        public ServiceResultModel Write(Guid id, HistoryLog history, Account? context)
        {
            if (context is null)
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

            history.Id = !Guid.Empty.Equals(history.Id) ? history.Id : id;
            history.ActorId = !Guid.Empty.Equals(history.ActorId) ? history.ActorId : context.Id;
            history.ModifiedDate = DateTime.Now;
            history.CreatedDate = DateTime.Now;
            history.IPAddress = history.IPAddress ?? context.IPAddress;
            history.Actor = history.Actor ?? context.UserName;
            history.ModifiedBy = history.ModifiedBy ?? context.FullName;
            history.CreatedBy = history.CreatedBy ?? context.FullName;
            history.ActionName = history.ActionName ?? string.Empty;
            history.Description = history.Description ?? string.Empty;

            return new ServiceResultModel
            {
                Success = true,
                //Data = _producer.Publish(new ObjectQueue
                //{
                //    QueueJobType = QueueJobTypeEnum.WRITELOG,
                //    DataType = "bphn.log.history",
                //    DataJson = JsonConvert.SerializeObject(history)
                //})
                Data = _historyLogRepository.Write(history).Result
            };
        }
    }
}
