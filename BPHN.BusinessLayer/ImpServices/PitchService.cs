using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class PitchService : BaseService, IPitchService
    {
        private readonly IPitchRepository _pitchRepository;
        private readonly IHistoryLogService _historyLogService;
        private readonly IFileService _fileService;
        private readonly ITimeFrameInfoRepository _timeFrameInfoRepository;
        private readonly INotificationService _notificationService;
        public PitchService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IPitchRepository pitchRepository, 
            IHistoryLogService historyLogService,
            IFileService fileService,
            INotificationService notificationService,
            ITimeFrameInfoRepository timeFrameInfoRepository) : base(serviceProvider, appSettings)
        {
            _pitchRepository = pitchRepository;
            _historyLogService = historyLogService;
            _fileService = fileService;
            _timeFrameInfoRepository = timeFrameInfoRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasInactive = true)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;
            var lstWhere = new List<WhereCondition>();

            if (!string.IsNullOrEmpty(accountId))
            {
                Guid id;
                Guid.TryParse(accountId, out id);

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

                if (!context.Id.Equals(id))
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NO_INTEGRITY,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                var hasPermission = await IsValidPermission(id, FunctionTypeEnum.VIEW_LIST_PITCH);
                if (!hasPermission)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.INVALID_ROLE,
                        Message = "Bạn không có quyền thực hiện chức năng này"
                    };
                }

                pageSize = int.MaxValue;


                lstWhere.Add(new WhereCondition()
                {
                    Column = "ManagerId",
                    Operator = "in",
                    Value = context.RelationIds.ToArray()
                });
            }

            if (!hasInactive || string.IsNullOrEmpty(accountId))
            {
                lstWhere.Add(new WhereCondition()
                {
                    Column = "Status",
                    Operator = "=",
                    Value = ActiveStatusEnum.ACTIVE.ToString()
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
            Pitch? data = null;
            if (string.IsNullOrEmpty(id))
            {
                data = new Pitch();
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
                var key = _cacheService.GetKeyCache(id, EntityEnum.PITCH);
                var cacheResult = await _cacheService.GetAsync(key);
                if(!string.IsNullOrWhiteSpace(cacheResult))
                {
                    data = JsonConvert.DeserializeObject<Pitch>(cacheResult);
                }
                else
                {
                    data = await _pitchRepository.GetById(id);
                    if(data != null)
                    {
                        data.TimeFrameInfos = await _timeFrameInfoRepository.GetByPitchId(data.Id);
                        await _cacheService.SetAsync(key, JsonConvert.SerializeObject(data));
                    }
                }

                if (data == null)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = "Không lấy được thông tin sân bóng. Vui lòng kiểm tra lại"
                    };
                }

                if (!string.IsNullOrEmpty(data.NameDetails))
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
                Guid id;
                Guid.TryParse(accountId, out id);

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

                if(!context.Id.Equals(id))
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NO_INTEGRITY,
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    };
                }

                var hasPermission = await IsValidPermission(id, FunctionTypeEnum.VIEW_LIST_PITCH);
                if (!hasPermission)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.INVALID_ROLE,
                        Message = "Bạn không có quyền thực hiện chức năng này"
                    };
                }

                pageSize = int.MaxValue;
                lstWhere.Add(new WhereCondition()
                {
                    Column = "ManagerId",
                    Operator = "in",
                    Value = context.RelationIds.ToArray()
                });
            }
            
            if(!hasInactive || string.IsNullOrEmpty(accountId))
            {
                lstWhere.Add(new WhereCondition()
                {
                    Column = "Status",
                    Operator = "=",
                    Value = ActiveStatusEnum.ACTIVE.ToString()
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
            var lstFrameInfo = await _timeFrameInfoRepository.GetByListPitchId(resultPaging.Select(item => item.Id).ToList());
            var dicFrame = new Dictionary<Guid, List<TimeFrameInfo>>();
            var now = DateTime.Now;
            foreach (var frame in lstFrameInfo)
            {
                frame.TimeBegin = new DateTime(now.Year, now.Month, now.Day, frame.TimeBegin.Hour, frame.TimeBegin.Minute, 0);
                frame.TimeEnd = new DateTime(now.Year, now.Month, now.Day, frame.TimeEnd.Hour, frame.TimeEnd.Minute, 0);
                if (dicFrame.ContainsKey(frame.PitchId))
                {
                    var currentFrame = dicFrame[frame.PitchId];
                    currentFrame.Add(frame);
                    dicFrame[frame.PitchId] = currentFrame;
                }
                else
                {
                    dicFrame.Add(frame.PitchId, new List<TimeFrameInfo>() { frame });
                }
            }
            
            resultPaging = resultPaging.Select(item =>
            {
                item.AvatarUrl = (string)(_fileService.GetLinkFile(item.Id.ToString()).Data ?? "");
                item.TimeFrameInfos = hasDetail && dicFrame.ContainsKey(item.Id) ? dicFrame[item.Id] : new List<TimeFrameInfo>();
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.ADD_PITCH);
            if(!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
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
                var notification = _notificationService.Insert<Pitch>(context, NotificationTypeEnum.ADD_PITCH, pitch);
                Thread thread = new Thread(delegate ()
                {
                    var historyLogId = Guid.NewGuid();
                    _historyLogService.Write(new HistoryLog()
                    {
                        Id = historyLogId,
                        IPAddress = context.IPAddress,
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.INSERT,
                        ActionName = string.Empty,
                        Entity = "Sân bóng",
                        Description = BuildLinkDescription(historyLogId),
                        Data = new HistoryLogDescription()
                        {
                            ModelId = pitch.Id,
                            NewData = JsonConvert.SerializeObject(pitch)
                        }
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.EDIT_PITCH);
            if (!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
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

            var oldPitch = await _pitchRepository.GetById(pitch.Id.ToString());
            var updateResult = await _pitchRepository.Update(pitch);

            if (updateResult)
            {
                var key = _cacheService.GetKeyCache(pitch.Id.ToString(), EntityEnum.PITCH);
                await _cacheService.RemoveAsync(key);

                var notification = _notificationService.Insert<Pitch>(context, NotificationTypeEnum.EDIT_PITCH, pitch);
                Thread thread = new Thread(delegate ()
                {
                    var historyLogId = Guid.NewGuid();
                    _historyLogService.Write(new HistoryLog()
                    {
                        Id = historyLogId,
                        IPAddress = context.IPAddress,
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.UPDATE,
                        ActionName = string.Empty,
                        Entity = "Sân bóng",
                        Description = BuildLinkDescription(historyLogId),
                        Data = new HistoryLogDescription()
                        {
                            ModelId = pitch.Id,
                            OldData = JsonConvert.SerializeObject(oldPitch),
                            NewData = JsonConvert.SerializeObject(pitch)
                        }
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
