using InventoryManagementSystem.Service.Interface;
using System.Security.Claims;

namespace InventoryManagementSystem.Middlewares
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                var roleIdClaim = user.FindFirst("RoleId")?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                    currentUser.UserId = long.Parse(userIdClaim);


                if (!string.IsNullOrEmpty(roleIdClaim) && int.TryParse(roleIdClaim, out var roleId))
                    currentUser.RoleId = roleId;
            }

            await _next(context);
        }
    }
}
