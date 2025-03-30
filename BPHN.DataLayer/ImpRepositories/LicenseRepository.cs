using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class LicenseRepository : BaseRepository, ILicenseRepository
    {
        public LicenseRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<License> Get(Guid accountId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var license = (await connection.QueryFirstOrDefaultAsync<License>(Query.LICENSE__GET, new Dictionary<string, object>
                {
                    { "@accountId", accountId }
                }));
                return license;
            }
        }

        public async Task<bool> Insert(License data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(Query.LICENSE__INSERT, new Dictionary<string, object?>
                {
                    { "@id", data.Id },
                    { "@accountId", data.AccountId },
                    { "@type", data.Type },
                    { "@maxInvoices", data.MaxInvoices },
                    { "@maxDraftInvoices", data.MaxDraftInvoices },
                    { "@expireTime", data.ExpireTime },
                    { "@createdDate", data.CreatedDate },
                    { "@createdBy", data.CreatedBy },
                    { "@modifiedBy", data.ModifiedBy },
                    { "@modifiedDate", data.ModifiedDate }
                });
                if (affect == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> Update(License data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(Query.LICENSE__UPDATE, new Dictionary<string, object?>
                {
                    { "@id", data.Id },
                    { "@type", data.Type },
                    { "@maxInvoices", data.MaxInvoices },
                    { "@expireTime", data.ExpireTime },
                    { "@modifiedBy", data.ModifiedBy },
                    { "@modifiedDate", data.ModifiedDate }
                });
                if (affect == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
