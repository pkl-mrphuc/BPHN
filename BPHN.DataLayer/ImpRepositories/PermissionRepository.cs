using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class PermissionRepository : BaseRepository, IPermissionRepository
    {
        public PermissionRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<IEnumerable<Permission>> GetPermissions(Guid accountId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                var lstPermission = await connection.QueryAsync<Permission>(Query.PERMISSION__GET_ALL, new Dictionary<string, object?>
                {
                    { "@accountId", accountId }
                });
                return lstPermission ?? Enumerable.Empty<Permission>();
            }
        }

        public async Task<bool> Save(Guid accountId, IEnumerable<Permission> permissions)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                using (var tranction = connection.BeginTransaction())
                {
                    var dic = new Dictionary<string, object?>
                    {
                        { "@accountId", accountId }
                    };
                    var affect = await connection.ExecuteAsync(Query.PERMISSION__DELETE, dic, tranction);

                    dic = new Dictionary<string, object?>();
                    var rows = new List<string>();
                    for (int i = 0; i < permissions.Count(); i++)
                    {
                        var permission = permissions.ElementAt(i);
                        dic.Add($"@id{i}", permission.Id);
                        dic.Add($"@accountId{i}", permission.AccountId);
                        dic.Add($"@functionType{i}", permission.FunctionType);
                        dic.Add($"@allow{i}", permission.Allow);
                        dic.Add($"@createdDate{i}", permission.CreatedDate);
                        dic.Add($"@createdBy{i}", permission.CreatedBy);
                        dic.Add($"@modifiedDate{i}", permission.ModifiedDate);
                        dic.Add($"@modifiedBy{i}", permission.ModifiedBy);
                        rows.Add($"(@id{i}, @accountId{i}, @functionType{i}, @allow{i}, @createdDate{i}, @createdBy{i}, @modifiedDate{i}, @modifiedBy{i})");
                    }

                    var query = $"insert into permissions(Id, AccountId, FunctionType, Allow, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values {string.Join(",", rows)}";
                    affect = await connection.ExecuteAsync(query, dic, tranction);
                    if (affect <= 0)
                    {
                        tranction.Rollback();
                    }
                    else
                    {
                        tranction.Commit();
                    }
                    return affect > 0;
                }
            }
        }
    }
}
