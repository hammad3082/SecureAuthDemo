using Microsoft.AspNetCore.SignalR;
using SecureAuthDemo.Services.Presence;

namespace SecureAuthDemo.Hubs
{
    public class OnlineUserHub : Hub
    {
        private readonly IPresenceService _presenceService;

        public OnlineUserHub(IPresenceService presenceService)
        {
            _presenceService = presenceService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var browserId = httpContext?.Request.Query["browserId"].ToString();

            if (!string.IsNullOrEmpty(browserId))
            {
                await _presenceService.UserConnectedAsync(browserId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var browserId = httpContext?.Request.Query["browserId"].ToString();

            await _presenceService.UserDisconnectedAsync(browserId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
