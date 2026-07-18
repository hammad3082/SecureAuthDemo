using SecureAuthDemo.Entities;
using System.Security.Claims;

namespace SecureAuthDemo.Services.Infrastructure
{
    public class UserContextAccessor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserContextAccessor(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void SetCurrentUser(User user)
        {
            var context = _contextAccessor.HttpContext;
            if (context == null) return;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, "ManualLoginTracking");
            context.User = new ClaimsPrincipal(identity);
        }
    }
}
