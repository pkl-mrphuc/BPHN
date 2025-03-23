using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class OverviewRepository : BaseRepository, IOverviewRepository
    {
        public OverviewRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }
    }
}
