using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface ILocationService
    {
        Task<ServiceResultModel> GetLocations(string txtSearch, int? provinceId, int? districtId, int? wardId);
    }
}
