using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<WsReceiveService> _hubContext;
        public NotificationService(
            IServiceProvider provider, 
            IOptions<AppSettings> appSettings,
            INotificationRepository notificationRepository, 
            IHubContext<WsReceiveService> hubContext) : base(provider, appSettings)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task<ServiceResultModel> GetTopFiveNewNotifications()
        {
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

            var lstWhere = new List<WhereCondition>()
            {
                new WhereCondition()
                {
                    Column = "AccountId",
                    Operator = "in",
                    Value = context.RelationIds.ToArray()
                }
            };

            return new ServiceResultModel()
            {
                Success = true,
                Data = await _notificationRepository.GetTopFiveNewNotifications(lstWhere)
            };
        }

        public async Task PushNotification(Account context, NotificationTypeEnum type)
        {
            var notification = new Notification()
            {
                Id = Guid.NewGuid(),
                AccountId = context.Id,
                Subject = "subject",
                Content = "content",
                NotificationType = (int)type,
                CreatedBy = context.FullName,
                CreatedDate = DateTime.Now,
                ModifiedBy = context.FullName,
                ModifiedDate = DateTime.Now
            };

            await _hubContext.Clients.All.SendAsync("PushNotificationToBellIcon", JsonConvert.SerializeObject(notification));

            var thread = new Thread(async delegate ()
            {
                await _notificationRepository.Insert(notification);
            });
            thread.Start();
        }
    }
}
