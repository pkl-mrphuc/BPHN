using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;

namespace BPHN.BusinessLayer.ImpServices
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
    }
}
