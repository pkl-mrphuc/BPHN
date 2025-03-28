using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class LocationService : BaseService, ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        public LocationService(IServiceProvider provider, IOptions<AppSettings> appSettings, ILocationRepository locationRepository) : base(provider, appSettings)
        {
            _locationRepository = locationRepository;
        }

        public async Task<ServiceResultModel> GetLocations(string txtSearch, int? provinceId, int? districtId, int? wardId)
        {
            return new ServiceResultModel
            {
                Success = true,
                Data = true
            };
        }
    }
}
