using AutoMapper;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter]
    public class BaseController : ControllerBase
    {
        protected readonly IMapper _mapper;

        public BaseController(IServiceProvider provider)
        {
            _mapper = provider.GetRequiredService<IMapper>();
        }
    }
}
