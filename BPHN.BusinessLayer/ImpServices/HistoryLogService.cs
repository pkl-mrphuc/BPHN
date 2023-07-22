using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.BusinessLayer.ImpServices
{
    public class HistoryLogService : IHistoryLogService
    {
        private readonly IHistoryLogRepository _historyLogRepository;
        private readonly IContextService _contextService;
        public HistoryLogService(IHistoryLogRepository historyLogRepository, IContextService contextService)
        {
            _historyLogRepository = historyLogRepository;
            _contextService = contextService;
        }
        public async Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

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


            var where = new List<WhereCondition>();
            where.Add(new WhereCondition()
            {
                Column = "ActorId",
                Operator = "=",
                Value = context.Id.ToString()
            });
            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                where.Add(new WhereCondition()
                {
                    Column = "ActionName",
                    Operator = "like",
                    Value = $"%{txtSearch}%"
                });
            }

            var resultCountPaging = await _historyLogRepository.GetCountPaging(pageIndex, pageSize, where);
            return new ServiceResultModel()
            {
                 Success = true,
                 Data = resultCountPaging
            };
        }

        public async Task<ServiceResultModel> GetDescription(string historyLogId)
        {
            return new ServiceResultModel()
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
            if (context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }


            var where = new List<WhereCondition>();
            where.Add(new WhereCondition()
            {
                Column = "ActorId",
                Operator = "=",
                Value = context.Id.ToString()
            });
            if(!string.IsNullOrWhiteSpace(txtSearch))
            {
                where.Add(new WhereCondition()
                {
                    Column = "ActionName",
                    Operator = "like",
                    Value = $"%{txtSearch}%"
                });
            }
            var resultPaging = await _historyLogRepository.GetPaging(pageIndex, pageSize, where);
            return new ServiceResultModel()
            {
                Success = true,
                Data = resultPaging
            };
        }

        public async Task<ServiceResultModel> Write(HistoryLog history, Account? context)
        {

            if(context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }

            switch (history.ActionType)
            {
                case ActionEnum.LOGIN:
                    history.ActionName = "Đăng nhập";
                    break;
                case ActionEnum.REGISTER_ACCOUNT:
                    history.ActionName = "Đăng ký tài khoản";
                    break;
                case ActionEnum.SEND_RESET_PASSWORD:
                    history.ActionName = "Gửi yêu cầu quên mật khẩu";
                    break;
                case ActionEnum.SUBMIT_RESET_PASSWORD:
                    history.ActionName = "Đổi mật khẩu mới";
                    break;
                case ActionEnum.SAVE:
                    history.ActionName = "Lưu";
                    break;
                case ActionEnum.INSERT:
                    history.ActionName = "Thêm mới";
                    break;
                case ActionEnum.UPDATE:
                    history.ActionName = "Cập nhật";
                    break;
            }

            history.ModifiedDate = DateTime.Now;
            history.ModifiedBy = context.FullName;
            history.CreatedBy = context.FullName;
            history.CreatedDate = DateTime.Now;
            history.Id = history.Id.Equals(Guid.Empty) ? Guid.NewGuid() : history.Id;

            return new ServiceResultModel()
            {
                Success = true,
                Data = await _historyLogRepository.Write(history)
            };
        }
    }
}
