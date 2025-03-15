using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class InvoiceRepository : BaseRepository, IInvoiceRepository
    {
        public InvoiceRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<Invoice> GetById(Guid id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var invoice = (await connection.QueryFirstOrDefaultAsync<Invoice>(Query.INVOICE__GET_BY_ID, new Dictionary<string, object>
                {
                    { "@id", id }
                }));
                return invoice;
            }
        }

        public async Task<IEnumerable<Invoice>> GetInvoices(Guid accountId, string txtSearch, string status, int? customerType, DateTime? date, int? paymentType)
        {
            var conditions = new List<WhereCondition>
            {
                new WhereCondition
                {
                    Column = "AccountId",
                    Operator = "=",
                    Value = accountId.ToString(),
                }
            };
            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "CustomerName",
                    Operator = "like",
                    Value = $"%{txtSearch}%",
                });
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Status",
                    Operator = "=",
                    Value = status,
                });
            }
            if (customerType.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "CustomerType",
                    Operator = "=",
                    Value = customerType.Value,
                });
            }
            if (date.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "Date",
                    Operator = ">=",
                    Value = date.Value.ToString("yyyy-MM-dd 00:00:00"),
                });
                conditions.Add(new WhereCondition
                {
                    Column = "Date",
                    Operator = "<=",
                    Value = date.Value.ToString("yyyy-MM-dd 23:59:59")
                });
            }
            if (paymentType.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "PaymentType",
                    Operator = "=",
                    Value = paymentType.Value,
                });
            }
            var where = BuildWhere(Query.INVOICE__GET_MANY, conditions);
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var invoices = await connection.QueryAsync<Invoice>(where.query, where.param);
                return invoices ?? Enumerable.Empty<Invoice>();
            }
        }

        public async Task<bool> Insert(Invoice data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = (await connection.ExecuteAsync(Query.INVOICE__INSERT, new Dictionary<string, object?>
                {
                    { "@id", data.Id },
                    { "@accountId", data.AccountId },
                    { "@customerType", (int)data.CustomerType },
                    { "@customerName", data.CustomerName },
                    { "@customerPhone", data.CustomerPhone },
                    { "@paymentType", (int)data.PaymentType },
                    { "@total", data.Total },
                    { "@date", data.Date },
                    { "@status", data.Status },
                    { "@detail", data.Detail },
                    { "@modifiedBy", data.ModifiedBy },
                    { "@modifiedDate", data.ModifiedDate },
                    { "@createdBy", data.CreatedBy },
                    { "@createdDate", data.CreatedDate }
                }));
                if (affect == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> Update(Invoice data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = (await connection.ExecuteAsync(Query.INVOICE__UPDATE, new Dictionary<string, object?>
                {
                    { "@id", data.Id },
                    { "@customerType", (int)data.CustomerType },
                    { "@customerName", data.CustomerName },
                    { "@customerPhone", data.CustomerPhone },
                    { "@paymentType", (int)data.PaymentType },
                    { "@total", data.Total },
                    { "@date", data.Date },
                    { "@status", data.Status },
                    { "@detail", data.Detail },
                    { "@modifiedBy", data.ModifiedBy },
                    { "@modifiedDate", data.ModifiedDate }
                }));
                if (affect == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
