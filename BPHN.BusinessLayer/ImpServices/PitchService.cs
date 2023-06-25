using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.BusinessLayer.ImpServices
{
    public class PitchService : BaseService, IPitchService
    {
        private readonly IPitchRepository _pitchRepository;
        private readonly IContextService _contextService;
        private readonly IHistoryLogService _historyLogService;
        private readonly IFileService _fileService;
        private readonly ITimeFrameInfoRepository _timeFrameInfoRepository;
        public PitchService(IPitchRepository pitchRepository, 
            IContextService contextService,
            IHistoryLogService historyLogService,
            IFileService fileService,
            ITimeFrameInfoRepository timeFrameInfoRepository)
        {
            _pitchRepository = pitchRepository;
            _contextService = contextService;
            _historyLogService = historyLogService;
            _fileService = fileService;
            _timeFrameInfoRepository = timeFrameInfoRepository;
        }

        public async Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasInactive = true)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var lstWhere = new List<WhereCondition>();

            if (!string.IsNullOrEmpty(accountId))
            {
                pageSize = int.MaxValue;

                lstWhere.Add(new WhereCondition()
                {
                    Column = "ManagerId",
                    Operator = "=",
                    Value = accountId
                });

                if (!hasInactive)
                {
                    lstWhere.Add(new WhereCondition()
                    {
                        Column = "Status",
                        Operator = "=",
                        Value = "ACTIVE"
                    });
                }
            }
            else
            {
                lstWhere.Add(new WhereCondition()
                {
                    Column = "Status",
                    Operator = "=",
                    Value = "ACTIVE"
                });
            }

            if (!string.IsNullOrEmpty(txtSearch))
            {
                lstWhere.Add(new WhereCondition()
                {
                    Column = "Name",
                    Operator = "like",
                    Value = $"%{txtSearch}%"
                });
            }

            var resultCountPaging = await _pitchRepository.GetCountPaging(pageIndex, pageSize, lstWhere);
            return new ServiceResultModel()
            {
                Success = true,
                Data = resultCountPaging
            };
        }

        public async Task<ServiceResultModel> GetInstance(string id)
        {
            var data = new Pitch();
            if (string.IsNullOrEmpty(id))
            {
                data.Id = Guid.NewGuid();
                var timeFrameInfos = new List<TimeFrameInfo>();
                for (int i = 0; i < data.TimeSlotPerDay; i++)
                {
                    var timeBegin = DateTime.Now;
                    var timeEnd = DateTime.Now;
                    var timeBeginSpan = new TimeSpan(timeBegin.Hour, timeBegin.Minute, 0);
                    timeBegin = timeBegin.Date.Add(timeBeginSpan);
                    timeEnd = timeBegin.AddMinutes(data.MinutesPerMatch);

                    timeFrameInfos.Add(new TimeFrameInfo()
                    {
                        Id = Guid.NewGuid(),
                        SortOrder = i + 1,
                        Name = string.Format("Khung {0}", i + 1),
                        Price = 0,
                        TimeBegin = timeBegin,
                        TimeEnd = timeEnd,
                        TimeBeginTick = timeBegin.Ticks,
                        TimeEndTick = timeEnd.Ticks
                    });
                }
                data.TimeFrameInfos = timeFrameInfos;
                data.TimeFrameInfoIds = string.Join(";", timeFrameInfos.Select(item => item.Id).ToArray());

                var lstsNameDetails = new List<string>();
                for (int i = 0; i < data.Quantity; i++)
                {
                    lstsNameDetails.Add(string.Format("Sân {0}", i + 1));
                }
                data.ListNameDetails = lstsNameDetails;
                data.NameDetails = string.Join(";", lstsNameDetails.Select(item => item).ToArray());
            }
            else
            {
                data = await _pitchRepository.GetById(id);

                if(data == null)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = "Không lấy được thông tin sân bóng. Vui lòng kiểm tra lại"
                    };
                }


                if(!string.IsNullOrEmpty(data.NameDetails))
                {
                    var lstNameDetails = data.NameDetails.Split(";").ToList();
                    data.ListNameDetails = lstNameDetails;
                }

                if (data.TimeSlotPerDay != data.TimeFrameInfos.Count ||
                    data.Quantity != data.ListNameDetails.Count)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NO_INTEGRITY,
                        Message = "Dữ liệu không toàn vẹn. Vui lòng kiểm tra lại"
                    };
                }

                var now = DateTime.Now;
                for (int i = 0; i < data.TimeFrameInfos.Count; i++)
                {
                    var item = data.TimeFrameInfos[i];
                    item.TimeBegin = new DateTime(now.Year, now.Month, now.Day, item.TimeBegin.Hour, item.TimeBegin.Minute, 0);
                    item.TimeEnd = new DateTime(now.Year, now.Month, now.Day, item.TimeEnd.Hour, item.TimeEnd.Minute, 0);
                    item.TimeBeginTick = item.TimeBegin.Ticks;
                    item.TimeEndTick = item.TimeEnd.Ticks;
                }
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = data
            };
        }

        public async Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasDetail = false, bool hasInactive = true)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var lstWhere = new List<WhereCondition>();

            if(!string.IsNullOrEmpty(accountId))
            {
                pageSize = int.MaxValue;

                lstWhere.Add(new WhereCondition()
                {
                    Column = "ManagerId",
                    Operator = "=",
                    Value = accountId
                });

                if(!hasInactive)
                {
                    lstWhere.Add(new WhereCondition()
                    {
                        Column = "Status",
                        Operator = "=",
                        Value = "ACTIVE"
                    });
                }
            }
            else
            {
                lstWhere.Add(new WhereCondition()
                {
                    Column = "Status",
                    Operator = "=",
                    Value = "ACTIVE"
                });
            }

            if(!string.IsNullOrEmpty(txtSearch))
            {
                lstWhere.Add(new WhereCondition()
                {
                    Column = "Name",
                    Operator = "like",
                    Value = $"%{txtSearch}%"
                });
            }

            var resultPaging = await _pitchRepository.GetPaging(pageIndex, pageSize, lstWhere);
            resultPaging = resultPaging.Select(item =>
            {
                item.AvatarUrl = (string)(_fileService.GetLinkFile(item.Id.ToString()).Data ?? "");
                item.TimeFrameInfos = hasDetail ? _timeFrameInfoRepository.GetByPitchId(item.Id).Result : new List<TimeFrameInfo>();
                return item;
            }).ToList();

            return new ServiceResultModel()
            {
                Success = true,
                Data = resultPaging
            };
        }

        public async Task<ServiceResultModel> Insert(Pitch pitch)
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

            if(pitch == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            var isValid = ValidateModelByAttribute(pitch, new List<string>());
            
            if( !isValid || pitch.TimeFrameInfos.Count == 0 || pitch.ListNameDetails.Count == 0)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            pitch.CreatedBy = context.FullName;
            pitch.CreatedDate = DateTime.Now;
            pitch.ModifiedBy = context.FullName;
            pitch.ModifiedDate = DateTime.Now;
            pitch.ManagerId = context.Id;

            pitch.TimeFrameInfos = pitch.TimeFrameInfos.Select(item =>
            {
                item.PitchId = pitch.Id;
                item.Id = item.Id.Equals(Guid.Empty) ? Guid.NewGuid() : item.Id;
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = context.FullName;
                item.ModifiedBy = context.FullName;
                item.TimeBegin = new DateTime(item.TimeBeginTick);
                item.TimeEnd = new DateTime(item.TimeEndTick);
                return item;
            }).ToList();
            pitch.NameDetails = string.Join(";", pitch.ListNameDetails.ToArray());

            var insertResult = await _pitchRepository.Insert(pitch);

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
                        Entity = "Sân bóng",
                        Description = BuildDescriptionForHistoryLog<Pitch>(null, pitch),
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

        public async Task<ServiceResultModel> Update(Pitch pitch)
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

            if (pitch == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            var isValid = ValidateModelByAttribute(pitch, new List<string>());

            if (!isValid || pitch.TimeFrameInfos.Count == 0 || pitch.ListNameDetails.Count == 0)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            pitch.ModifiedBy = context.FullName;
            pitch.ModifiedDate = DateTime.Now;
            pitch.NameDetails = string.Join(";", pitch.ListNameDetails.ToArray());

            pitch.TimeFrameInfos = pitch.TimeFrameInfos.Select(item =>
            {
                item.PitchId = pitch.Id;
                item.Id = item.Id.Equals(Guid.Empty) ? Guid.NewGuid() : item.Id;
                item.CreatedDate = DateTime.Now;
                item.ModifiedDate = DateTime.Now;
                item.CreatedBy = context.FullName;
                item.ModifiedBy = context.FullName;
                item.TimeBegin = new DateTime(item.TimeBeginTick);
                item.TimeEnd = new DateTime(item.TimeEndTick);
                return item;
            }).ToList();
            

            var updateResult = await _pitchRepository.Update(pitch);

            if (updateResult)
            {
                Thread thread = new Thread(delegate ()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.UPDATE,
                        ActionName = string.Empty,
                        Entity = "Sân bóng",
                        Description = BuildDescriptionForHistoryLog<Pitch>(null, pitch),
                    }, context);
                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = updateResult
            };
        }
    }
}
