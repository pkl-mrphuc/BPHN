using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Serilog;

namespace BPHN.WebAPI.Controllers
{
    [AllowAnonymous]
    public class MailTemplatesController : BaseController
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _provider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly ICacheService _cacheService;
        private string _folderDir = string.Empty;

        public MailTemplatesController(IServiceProvider provider) : base(provider)
        {
            _provider = provider;
            _viewEngine = provider.GetRequiredService<IRazorViewEngine>();
            _tempDataProvider = provider.GetRequiredService<ITempDataProvider>();
            _cacheService = provider.GetRequiredService<ICacheService>();
            _folderDir = Path.Combine("Views", "MailTemplates");
        }

        [Route("forgot-password")]
        [HttpPost]
        public async Task<IActionResult> GetForgotPasswordBody([FromBody] MailVm<MailForgotPasswordVm> request)
        {
            Log.Debug($"MailTemplate/GetForgotPasswordBody start: {JsonConvert.SerializeObject(request)}");
            var pathView = Path.Combine(_folderDir, "ForgotPassword.cshtml");
            var source = await RenderAsync<MailForgotPasswordVm>(pathView, request.Model, request.ViewBag);
            return Ok(source);
        }

        [Route("set-password")]
        [HttpPost]
        public async Task<IActionResult> GetSetPasswordBody([FromBody] MailVm<MailSetPasswordVm> request)
        {
            Log.Debug($"MailTemplate/GetSetPasswordBody start: {JsonConvert.SerializeObject(request)}");
            var pathView = Path.Combine(_folderDir, "SetPassword.cshtml");
            var source = await RenderAsync<MailSetPasswordVm>(pathView, request.Model, request.ViewBag);
            return Ok(source);
        }

        [Route("approval-booking")]
        [HttpPost]
        public async Task<IActionResult> GetApprovalBookingBody([FromBody] MailVm<MailApprovalBookingVm> request)
        {
            Log.Debug($"MailTemplate/GetApprovalBookingBody start: {JsonConvert.SerializeObject(request)}");
            var pathView = Path.Combine(_folderDir, "ApprovalBooking.cshtml");
            var source = await RenderAsync<MailApprovalBookingVm>(pathView, request.Model, request.ViewBag);
            return Ok(source);
        }

        [Route("decline-booking")]
        [HttpPost]
        public async Task<IActionResult> GetDeclineBookingBody([FromBody] MailVm<MailDeclineBookingVm> request)
        {
            Log.Debug($"MailTemplate/GetDeclineBookingBody start: {JsonConvert.SerializeObject(request)}");
            var pathView = Path.Combine(_folderDir, "DeclineBooking.cshtml");
            var source = await RenderAsync<MailDeclineBookingVm>(pathView, request.Model, request.ViewBag);
            return Ok(source);
        }

        private async Task<string> RenderAsync<T>(string pathView, T model, dynamic viewBag)
        {
            if (string.IsNullOrWhiteSpace(pathView) || !System.IO.File.Exists(pathView))
            {
                return "";
            }
            await using StringWriter stringWriter = new();
            var viewResult = _viewEngine.GetView(null, pathView, true);
            if (viewResult.Success)
            {
                var view = viewResult.View;

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
