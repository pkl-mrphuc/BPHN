using BPHN.BusinessLayer.IServices;

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
                GetContext(context, accountService, token);
            }

            await _next(context);
        }

        private void GetContext(HttpContext context, IAccountService accountService, string token)
        {
            var tokenResult = accountService.ValidateToken(token);
            if (tokenResult is not null && tokenResult.Success && tokenResult.Data is not null)
            {
                if (!context.Items.ContainsKey("User") || context.Items["User"] is null)
                {
                    context.Items["User"] = tokenResult.Data;
                }
            }
        }
    }
}
