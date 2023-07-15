using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetPermissions(Guid accountId);
        Task<bool> Save(List<Permission> permissions);
    }
}
