using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
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

        public async Task<IEnumerable<Item>> GetItems(Guid accountId, string txtSearch, string status, string code, string unit, string quantity)
        {
            var conditions = new List<WhereCondition>
            {
                new WhereCondition
                {
                    Column = "AccountId",
                    Operator = "=",
                    Value = accountId.ToString()
                }
            };
            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Name",
                    Operator = "=",
                    Value = txtSearch
                });
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Status",
                    Operator = "=",
                    Value = status
                });
            }
            if (!string.IsNullOrWhiteSpace(code))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Code",
                    Operator = "like",
                    Value = $"%{code}%"
                });
            }
            if (!string.IsNullOrWhiteSpace(unit))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Unit",
                    Operator = "like",
                    Value = $"%{unit}%"
                });
            }
            if (QuantityStatusEnum.AVAILABLE.ToString().Equals(quantity))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Quantity",
                    Operator = ">",
                    Value = 0
                });
            }
            if (QuantityStatusEnum.UNAVAILABLE.ToString().Equals(quantity))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Quantity",
                    Operator = "=",
                    Value = 0
                });
            }
            var where = BuildWhere(Query.ITEM__GET_MANY, conditions);
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var items = await connection.QueryAsync<Item>(where.query, where.param);
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

        public async Task UpdateQuantity(Guid accountId, IEnumerable<InvoiceItem> items)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                foreach (var item in items)
                {
                    await connection.ExecuteAsync(Query.ITEM__UPDATE_QUANTITY_BY_ID, new Dictionary<string, object>
                    {
                        { "@id", item.ItemId },
                        { "@quantity", item.Quantity },
                    });
                }
            }
        }
    }
}
