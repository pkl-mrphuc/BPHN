using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static K4os.Compression.LZ4.Engine.Pubternal;

namespace BPHN.BusinessLayer.ImpServices
{
    public class PitchService : BaseService, IPitchService
    {
        private readonly IPitchRepository _pitchRepository;
        private readonly IContextService _contextService;
        private readonly IHistoryLogService _historyLogService;
        public PitchService(IPitchRepository pitchRepository, 
            IContextService contextService,
            IHistoryLogService historyLogService)
        {
            _pitchRepository = pitchRepository;
            _contextService = contextService;
            _historyLogService = historyLogService; 
        }

        public ServiceResultModel GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var resultCountPaging = _pitchRepository.GetCountPaging(pageIndex, pageSize, txtSearch);
            return new ServiceResultModel()
            {
                Success = true,
                Data = resultCountPaging
            };
        }

        public ServiceResultModel GetInstance(string id)
        {
            var data = new Pitch();
            if (string.IsNullOrEmpty(id))
            {
                data.Id = Guid.NewGuid();
                var timeFrameInfos = new List<TimeFrameInfo>();
                for (int i = 0; i < data.TimeSlotPerDay; i++)
                {
                    var timeBegin = DateTime.Now;
                    var timeBeginSpan = new TimeSpan(timeBegin.Hour, timeBegin.Minute, 0);
                    timeBegin = timeBegin.Date.Add(timeBeginSpan);

                    timeFrameInfos.Add(new TimeFrameInfo()
                    {
                        Id = Guid.NewGuid(),
                        SortOrder = i + 1,
                        Name = string.Format("Khung {0}", i + 1),
                        Price = 0,
                        TimeBegin = timeBegin,
                        TimeEnd = timeBegin.AddMinutes(data.MinutesPerMatch)
                    });
                }
                data.TimeFrameInfos = timeFrameInfos;
                data.TimeFrameInfoIds = string.Join(";", timeFrameInfos.Select(item => item.Id).ToArray());
            }
            else
            {
                data = _pitchRepository.GetById(id);

                if(data == null)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = "Không lấy được thông tin sân bóng. Vui lòng kiểm tra lại"
                    };
                }

                if(data.TimeSlotPerDay != data.TimeFrameInfos.Count)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NO_INTEGRITY,
                        Message = "Dữ liệu không toàn vẹn. Vui lòng kiểm tra lại"
                    };
                }
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = data
            };
        }

        public ServiceResultModel GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var resultPaging = _pitchRepository.GetPaging(pageIndex, pageSize, txtSearch);
            return new ServiceResultModel()
            {
                Success = true,
                Data = resultPaging
            };
        }

        public ServiceResultModel Insert(Pitch pitch)
        {
            var isValid = ValidateModelByAttribute(pitch, new List<string>());
            
            if(!isValid || (pitch != null && pitch.TimeFrameInfos.Count == 0))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            var context = _contextService.GetContext();
            if(context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }

            pitch.ManagerId = context.Id;
            pitch.CreatedBy = context.FullName;
            pitch.CreatedDate = DateTime.Now;
            pitch.ModifiedBy = context.FullName;
            pitch.ModifiedDate = DateTime.Now;

            pitch.TimeFrameInfos = pitch.TimeFrameInfos.Select(item =>
            {
                item.PitchId = pitch.Id;
                item.Id = item.Id.Equals(Guid.Empty) ? Guid.NewGuid() : item.Id;
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = context.FullName;
                item.ModifiedBy = context.FullName;
                return item;
            }).ToList();

            var insertResult = _pitchRepository.Insert(pitch);

            if(insertResult)
            {
                Thread thread = new Thread(delegate ()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.INSERT,
                        ActionName = string.Empty,
                        Description = "thông tin sân bóng",
                    }, context);
                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = insertResult
            };
        }

        public ServiceResultModel Update(Pitch pitch)
        {
            throw new NotImplementedException();
        }
    }
}
