using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface ILicenseRepository
    {
        Task<License> Get(Guid accountId);
        Task<bool> Insert(License data);
        Task<bool> Update(License data);
    }
}
