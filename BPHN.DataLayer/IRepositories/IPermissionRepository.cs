using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetPermissions(Guid accountId);
        Task<bool> Save(Guid accountId, IEnumerable<Permission> permissions);
    }
}
