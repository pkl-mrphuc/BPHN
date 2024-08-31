using BPHN.BusinessLayer.IServices;
using System.IdentityModel.Tokens.Jwt;

namespace BPHN.WebAPI
{
    public class AuthenMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAccountService accountService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await GetContext(context, accountService, token);
            }

            await _next(context);
        }

        private async Task GetContext(HttpContext context, IAccountService accountService, string token)
        {
            var tokenResult = accountService.GetTokenInfo(token);
            if(tokenResult.Success && tokenResult.Data != null)
            {
                var jwtToken = (JwtSecurityToken)tokenResult.Data;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                var serviceResult = await accountService.GetById(userId);
                context.Items["User"] = serviceResult.Data;
            }
        }
    }
}
