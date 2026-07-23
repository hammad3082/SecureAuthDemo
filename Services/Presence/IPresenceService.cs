namespace SecureAuthDemo.Services.Presence
{
    public interface IPresenceService
    {
        Task UserConnectedAsync(string userId);
        Task UserDisconnectedAsync(string userId);
    }
}
