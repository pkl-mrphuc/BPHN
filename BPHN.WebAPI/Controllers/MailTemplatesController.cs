using BPHN.ModelLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BPHN.WebAPI.Controllers
{
    [AllowAnonymous]
    public class MailTemplatesController : BaseController
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _provider;
        private readonly ITempDataProvider _tempDataProvider;
        private string _folderDir = string.Empty;

        public MailTemplatesController(IServiceProvider provider)
        {
            _provider = provider;
            _viewEngine = provider.GetRequiredService<IRazorViewEngine>();
            _tempDataProvider = provider.GetRequiredService<ITempDataProvider>();
            _folderDir = Path.Combine("Views", "MailTemplates");
        }

        [Route("reset-password")]
        [HttpPost]
        public async Task<IActionResult> GetResetPasswordBody([FromBody] MailVm<MailResetPasswordVm> request)
        {
            string pathView = Path.Combine(_folderDir, "ResetPassword.cshtml");
            string source = await RenderAsync<MailResetPasswordVm>(pathView, request.Model, request.ViewBag);
            return Ok(source);
        }

        private async Task<string> RenderAsync<T>(string pathView, T model, dynamic viewBag)
        {
            if (string.IsNullOrEmpty(pathView) || !System.IO.File.Exists(pathView))
            {
                return "";
            }
            await using StringWriter stringWriter = new();
            var viewResult = _viewEngine.GetView(null, pathView, true);
            if (viewResult.Success)
            {
                IView view = viewResult.View;

                DefaultHttpContext defaultHttpContext = new()
                {
                    RequestServices = _provider
                };
                ActionContext actionContext = new(defaultHttpContext, new RouteData(), new ActionDescriptor());
                
                ViewDataDictionary viewDataDictionary = new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };
                if (viewBag != null)
                {
                    foreach (KeyValuePair<string, object> kv in viewBag as IDictionary<string, object>)
                    {
                        viewDataDictionary.Add(kv.Key, kv.Value);
                    }
                }
                TempDataDictionary tempDataDictionary = new(actionContext.HttpContext, _tempDataProvider);
                ViewContext viewContext = new(
                    actionContext,
                    view,
                    viewDataDictionary,
                    tempDataDictionary,
                    stringWriter,
                    new HtmlHelperOptions()
                );
                await view.RenderAsync(viewContext);
            }
            return stringWriter.ToString();
        }
    }
}
