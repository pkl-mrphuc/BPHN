using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class ItemRepository : BaseRepository, IItemRepository
    {
        public ItemRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<IEnumerable<Item>> GetAll(Guid accountId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var items = (await connection.QueryAsync<Item>(Query.ITEM__GET_ALL, new Dictionary<string, object>
                {
                    { "@accountId", accountId }
                }));
                return items?.ToList() ?? Enumerable.Empty<Item>();
            }
        }

        public async Task<Item> GetById(Guid id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var item = (await connection.QueryFirstOrDefaultAsync<Item>(Query.ITEM__GET_BY_ID, new Dictionary<string, object>
                {
                    { "@id", id }
                }));
                return item;
            }
        }

        public async Task<bool> Insert(Item data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(Query.ITEM__INSERT, new Dictionary<string, object?>
                {
                    { "@id", data.Id },
                    { "@accountId", data.AccountId },
                    { "@unit", data.Unit },
                    { "@status", data.Status },
                    { "@code", data.Code },
                    { "@name", data.Name },
                    { "@quantity", data.Quantity },
                    { "@salePrice", data.SalePrice },
                    { "@purchasePrice", data.PurchasePrice },
                    { "@modifiedBy", data.ModifiedBy },
                    { "@modifiedDate", data.ModifiedDate },
                    { "@createdBy", data.CreatedBy },
                    { "@createdDate", data.CreatedDate }
                });
                if (affect == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> Update(Item data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(Query.ITEM__UPDATE_BY_ID, new Dictionary<string, object?>
                {
                    { "@id", data.Id },
                    { "@status", data.Status },
                    { "@unit", data.Unit },
                    { "@code", data.Code },
                    { "@name", data.Name },
                    { "@quantity", data.Quantity },
                    { "@salePrice", data.SalePrice },
                    { "@purchasePrice", data.PurchasePrice },
                    { "@modifiedBy", data.ModifiedBy },
                    { "@modifiedDate", data.ModifiedDate },
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
