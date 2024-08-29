using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Microsoft.AspNetCore.Http;

namespace BPHN.BusinessLayer.ImpServices
{
    public class ContextService : IContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Account? GetContext()
        {
            var user = _httpContextAccessor.HttpContext?.Items["User"];
            if (user is not null)
            {
                var result = (Account)user;
                result.IPAddress = GetIPAddress();
                return result;
            }
            return null;
        }

        public string GetIPAddress() => _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
    }
}
