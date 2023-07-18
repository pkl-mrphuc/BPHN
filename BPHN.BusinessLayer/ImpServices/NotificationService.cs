using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.Extensions.Options;

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

        public ServiceResultModel Insert<T>(Account context, NotificationTypeEnum type, T model)
        {
            var notification = new Notification()
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

            var thread = new Thread(async delegate ()
            {
                await _notificationRepository.Insert(notification);
            });
            thread.Start();

            return new ServiceResultModel()
            {
                Success = true,
                Data = notification
            };
        }

        private string BuildSubject<T>(NotificationTypeEnum type, T model)
        {
            switch(type)
            {
                case NotificationTypeEnum.ADD_PITCH:
                case NotificationTypeEnum.EDIT_PITCH:
                    return "Sân bóng";
                case NotificationTypeEnum.ADD_BOOKING:
                case NotificationTypeEnum.EDIT_BOOKING:
                    return "Đặt sân";
                case NotificationTypeEnum.ADD_USER:
                case NotificationTypeEnum.EDIT_USER:
                    return "Tài khoản";
                default:
                    return "";
            }
        }

        private string BuildContent<T>(NotificationTypeEnum type, T model)
        {
            switch (type)
            {
                case NotificationTypeEnum.ADD_PITCH:
                    return $"Thêm mới sân bóng";
                case NotificationTypeEnum.EDIT_PITCH:
                    return $"Sửa thông tin sân bóng";
                case NotificationTypeEnum.ADD_BOOKING:
                    return $"Thêm mới thông tin đặt sân";
                case NotificationTypeEnum.EDIT_BOOKING:
                    return $"Sửa thông tin đặt sân";
                case NotificationTypeEnum.ADD_USER:
                    return $"Thêm mới tài khoản";
                case NotificationTypeEnum.EDIT_USER:
                    return $"Sửa thông tin tài khoản";
                default:
                    return "";
            }
        }
    }
}
