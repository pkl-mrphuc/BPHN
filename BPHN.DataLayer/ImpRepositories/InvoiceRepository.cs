using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
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

        public async Task<IEnumerable<Invoice>> GetInvoices(Guid accountId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var invoices = (await connection.QueryAsync<Invoice>(Query.INVOICE__GET_ALL, new Dictionary<string, object>
                {
                    { "@accountId", accountId }
                }));
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
