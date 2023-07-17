using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        public NotificationRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<List<Notification>> GetTopFiveNewNotifications(List<WhereCondition> where)
        {
            var whereQuery = BuildWhereQuery(where);
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object?>();
                var query = @$"select * from notifications where {whereQuery} order by CreatedDate desc limit 0, 5";
                for (int i = 0; i < where.Count; i++)
                {
                    dic.Add($"@where{i}", where[i].Value);
                }
                var result = await connection.QueryAsync<Notification>(query, dic);
                return result.ToList();
            }
        }

        public async Task<bool> Insert(Notification notification)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object?>();
                dic.Add("@id", notification.Id);
                dic.Add("@subject", notification.Subject);
                dic.Add("@content", notification.Content);
                dic.Add("@notificationType", notification.NotificationType);
                dic.Add("@accountId", notification.AccountId);
                dic.Add("@createdDate", notification.CreatedDate);
                dic.Add("@createdBy", notification.CreatedBy);
                dic.Add("@modifiedDate", notification.ModifiedDate);
                dic.Add("@imodifiedByd", notification.ModifiedBy);
                var query = @$"insert into notifications(Id, Subject, Content, NotificationType, AccountId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                value(@id, @subject, @content, @notificationType, @accountId, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
                var affect = await connection.ExecuteAsync(query, dic);
                return affect > 0 ? true : false;
            }
        }
    }
}
