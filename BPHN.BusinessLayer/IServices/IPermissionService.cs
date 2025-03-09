using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IPermissionService
    {
        Task<ServiceResultModel> GetPermissions(Guid accountId);
        Task<ServiceResultModel> SavePermissions(Guid accountId, IEnumerable<Permission> permissions);
        Task<IEnumerable<Permission>> GetAll(Guid accountId);
        IEnumerable<Permission> GetDefaultPermissions(Guid accountId, Account context);
        Task<bool> IsValidPermission(Guid accountId, FunctionTypeEnum functionType);
    }
}
