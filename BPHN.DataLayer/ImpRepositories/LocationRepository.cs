using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class LocationRepository : BaseRepository, ILocationRepository
    {
        public LocationRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }
        public Task<IEnumerable<Location>> GetLocations(string txtSearch, int? provinceId, int? districtId, int? wardId)
        {
            throw new NotImplementedException();
        }

    }
}
