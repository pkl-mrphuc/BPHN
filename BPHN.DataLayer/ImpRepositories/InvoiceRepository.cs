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
    }
}
