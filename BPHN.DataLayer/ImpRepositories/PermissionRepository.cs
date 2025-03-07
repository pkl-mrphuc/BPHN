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

        public async Task<bool> Save(IEnumerable<Permission> permissions)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                using (var tranction = connection.BeginTransaction())
                {
                    foreach (var permission in permissions)
                    {
                        var dic = new Dictionary<string, object?>();
                        dic.Add("@id", permission.Id);
                        dic.Add("@accountId", permission.AccountId);
                        dic.Add("@functionType", permission.FunctionType);
                        dic.Add("@allow", permission.Allow);
                        dic.Add("@createdDate", permission.CreatedDate);
                        dic.Add("@createdBy", permission.CreatedBy);
                        dic.Add("@modifiedDate", permission.ModifiedDate);
                        dic.Add("@modifiedBy", permission.ModifiedBy);
                        var query = @"set @idPermission = (select Id from permissions where AccountId = @accountId and Id = @id);
                                        set @idPermission = if(@idPermission is null, @id, @idPermission);
                                        insert into permissions(Id, AccountId, FunctionType, Allow, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                            value(@idPermission, @accountId, @functionType, @allow, @createdDate, @createdBy, @modifiedDate, @modifiedBy)
                                        on duplicate key update Allow = @allow, ModifiedDate = @modifiedDate, ModifiedBy = @modifiedBy";
                        var affect = await connection.ExecuteAsync(query, dic, tranction, commandType: System.Data.CommandType.Text);
                        if (affect == 0)
                        {
                            tranction.Rollback();
                            return false;
                        }
                    }
                    tranction.Commit();
                }
            }
            return true;
        }
    }
}
