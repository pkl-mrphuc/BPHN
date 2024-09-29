using AutoMapper;
using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter]
    public class BaseController : ControllerBase
    {
        protected readonly ILogService _logger;
        protected readonly IMapper _mapper;

        public BaseController(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<ILogService>();
            _mapper = provider.GetRequiredService<IMapper>();
        }
    }
}
