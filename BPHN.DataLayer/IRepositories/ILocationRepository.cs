using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetLocations(string txtSearch, int? provinceId, int? districtId, int? wardId);
    }
}
