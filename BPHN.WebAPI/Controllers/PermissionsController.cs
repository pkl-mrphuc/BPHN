using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    [ApiAuthorize]
    public class PermissionsController : BaseController
    {
        private readonly IPermissionService _permissionService;
        public PermissionsController(IServiceProvider provider)
        {
            _permissionService = provider.GetRequiredService<IPermissionService>();
        }
    }
}
