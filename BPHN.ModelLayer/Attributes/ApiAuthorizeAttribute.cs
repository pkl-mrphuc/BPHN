using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPHN.ModelLayer.Attributes
{
    public class ApiAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.Items["User"];
            if (user == null)
            {
                context.Result = new OkObjectResult(new ServiceResultModel()
                {
                    Success = false,
                    Message = "UnAuthorize"
                });
            }
        }
    }
}
