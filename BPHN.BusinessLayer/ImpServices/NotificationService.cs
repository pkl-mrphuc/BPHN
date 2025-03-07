using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(
            IServiceProvider provider,
            IOptions<AppSettings> appSettings,
            INotificationRepository notificationRepository) : base(provider, appSettings)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<ServiceResultModel> GetTopFiveNewNotifications()
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

            var lstWhere = new List<WhereCondition>
            {
                new WhereCondition
                {
                    Column = "AccountId",
                    Operator = "in",
                    Value = context.RelationIds.ToArray()
                }
            };

            var lstNotification = await _notificationRepository.GetTopFiveNewNotifications(lstWhere);

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<NotificationRespond>>(lstNotification)
            };
        }

        public async Task<ServiceResultModel> Insert<T>(Account context, NotificationTypeEnum type, T model)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                AccountId = context.Id,
                Subject = BuildSubject<T>(type, model),
                Content = BuildContent<T>(type, model),
                NotificationType = (int)type,
                CreatedBy = context.FullName,
                CreatedDate = DateTime.Now,
                ModifiedBy = context.FullName,
                ModifiedDate = DateTime.Now,
            };

            if (_appSettings is not null && !string.IsNullOrWhiteSpace(_appSettings.SignalrUrl))
            {
                var connection = new HubConnectionBuilder().WithUrl(new Uri(_appSettings.SignalrUrl)).Build();
                await connection.StartAsync();
                await connection.InvokeAsync(
                                                "PushNotification",
                                                context.RelationIds.Select(item => item.ToString()).ToList(),
                                                context.Id.ToString(),
                                                (int)type,
                                                JsonConvert.SerializeObject(model)
                                            );
            }

            await _notificationRepository.Insert(notification);

            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<NotificationRespond>(notification)
            };
        }

        private string BuildSubject<T>(NotificationTypeEnum type, T model)
        {
            switch (type)
            {
                case NotificationTypeEnum.INSERTPITCH:
                case NotificationTypeEnum.UPDATEPITCH:
                    return EntityEnum.PITCH.ToString();
                case NotificationTypeEnum.INSERTBOOKING:
                case NotificationTypeEnum.UPDATEMATCH:
                case NotificationTypeEnum.CANCELBOOKINGDETAIL:
                case NotificationTypeEnum.DECLINEBOOKING:
                case NotificationTypeEnum.APPROVALBOOKING:
                    return EntityEnum.BOOKING.ToString();
                case NotificationTypeEnum.CHANGEPERMISSION:
                case NotificationTypeEnum.INSERTACCOUNT:
                    return EntityEnum.ACCOUNT.ToString();
                default:
                    return string.Empty;
            }
        }

        private string BuildContent<T>(NotificationTypeEnum type, T model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}
