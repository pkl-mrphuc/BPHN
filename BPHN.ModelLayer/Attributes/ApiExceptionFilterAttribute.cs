using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPHN.ModelLayer.Attributes
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            context.Result = new OkObjectResult(new ServiceResultModel()
            {
                Success = false,
                Message = context.Exception.Message
            });
        }
    }
}
