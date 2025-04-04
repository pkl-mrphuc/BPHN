using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(IServiceProvider provider, IOptions<AppSettings> appSettings) : base(provider, appSettings)
        {

        }

        public async Task Insert<T>(Account context, NotificationTypeEnum type, T model)
        {
            try
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
                    await connection.InvokeAsync("PushNotification", context.RelationIds, context.Id, (int)type, JsonConvert.SerializeObject(notification));
                }
            }
            catch (Exception)
            {
                
            }
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
                case NotificationTypeEnum.INSERTINVOICE:
                case NotificationTypeEnum.UPDATEINVOICE:
                    return EntityEnum.INVOICE.ToString();
                case NotificationTypeEnum.INSERTSERVICE:
                case NotificationTypeEnum.UPDATESERVICE:
                    return EntityEnum.SERVICE.ToString();
                default:
                    return string.Empty;
            }
        }

        private string BuildContent<T>(NotificationTypeEnum type, T model)
        {
            switch(type)
            {
                case NotificationTypeEnum.CANCELBOOKINGDETAIL: return "";
                case NotificationTypeEnum.UPDATEMATCH: return "";
                case NotificationTypeEnum.INSERTBOOKING: return "";
                case NotificationTypeEnum.DECLINEBOOKING: return "";
                case NotificationTypeEnum.APPROVALBOOKING: return "";
                case NotificationTypeEnum.CHANGEPERMISSION: return "";
                case NotificationTypeEnum.INSERTPITCH: return "";
                case NotificationTypeEnum.UPDATEPITCH: return "";
                case NotificationTypeEnum.INSERTACCOUNT: return "";
                case NotificationTypeEnum.UPDATEACCOUNT: return "";
                case NotificationTypeEnum.INSERTSERVICE: return "";
                case NotificationTypeEnum.UPDATESERVICE: return "";
                case NotificationTypeEnum.INSERTINVOICE: return "";
                case NotificationTypeEnum.UPDATEINVOICE: return "";
                default: return string.Empty;
            }
        }
    }
}
