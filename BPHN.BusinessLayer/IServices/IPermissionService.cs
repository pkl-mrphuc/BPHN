using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IPermissionService
    {
        Task<ServiceResultModel> GetPermissions(Guid accountId);
        Task<ServiceResultModel> SavePermissions(Guid accountId, List<Permission> permissions);
        List<Permission> GetDefaultPermissions(Guid accountId, Account context);
    }
}
