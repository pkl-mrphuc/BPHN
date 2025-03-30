using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface ILicenseService
    {
        Task<bool> Insert(License data);
        Task<bool> Update(License data);
        Task<bool> CheckIsValid(Guid accountId);
    }
}
