using Microsoft.AspNetCore.SignalR;
using SecureAuthDemo.Hubs;
using SecureAuthDemo.Services.Cache;

namespace SecureAuthDemo.Services.Presence
{
    public class PresenceService : IPresenceService
    {
        private readonly ICacheService _cacheService;
        private readonly IHubContext<OnlineUserHub> _hubContext;

        private const string BrowserKeyPrefix = "browser:";
        private const string GlobalCountKey = "global_online_user_count";

        public PresenceService(ICacheService cacheService, IHubContext<OnlineUserHub> hubContext)
        {
            _cacheService = cacheService;
            _hubContext = hubContext;
        }
        public async Task UserConnectedAsync(string browserKey)
        {
            string cacheKey = $"{BrowserKeyPrefix}{browserKey}";

            string? currentTabsRaw = await _cacheService.GetAsync(cacheKey);

            int currentTabs = string.IsNullOrEmpty(currentTabsRaw) ? 0 : int.Parse(currentTabsRaw);

            if (currentTabs == 0)
            {
                await _cacheService.IncrementAsync(GlobalCountKey);
            }

            await _cacheService.IncrementAsync(cacheKey);

            string? globalCountRaw = await _cacheService.GetAsync(GlobalCountKey);
            long globalCount = string.IsNullOrEmpty(globalCountRaw) ? 0 : long.Parse(globalCountRaw);

            await _hubContext.Clients.All.SendAsync("UpdateOnlineCount", globalCount);
        }

        public async Task UserDisconnectedAsync(string browserKey)
        {
            string cacheKey = $"{BrowserKeyPrefix}{browserKey}";

            string? currentTabsRaw = await _cacheService.GetAsync(cacheKey);
            int currentTabs = string.IsNullOrEmpty(currentTabsRaw) ? 0 : int.Parse(currentTabsRaw);

            if (currentTabs > 0)
            {
                int updatedTabs = currentTabs - 1;

                if (updatedTabs == 0)
                {
                    await _cacheService.RemoveAsync(cacheKey);
                    await _cacheService.DecrementAsync(GlobalCountKey);
                }
                else
                {
                    await _cacheService.DecrementAsync(cacheKey);
                }
            }

            string? globalCountRaw = await _cacheService.GetAsync(GlobalCountKey);
            long globalCount = string.IsNullOrEmpty(globalCountRaw) ? 0 : long.Parse(globalCountRaw);

            if (globalCount < 0) globalCount = 0;

            await _hubContext.Clients.All.SendAsync("UpdateOnlineCount", globalCount);
        }
    }
}
