using Microsoft.AspNetCore.SignalR;

namespace Himu.Home.HttpApi.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Clients.All.SendAsync("RefreshMessage").GetAwaiter().GetResult();
            return base.OnConnectedAsync();
        }

        public async Task RequestRefreshMessage()
        {
            await Clients.All.SendAsync("RefreshMessage");
        }
    }
}
