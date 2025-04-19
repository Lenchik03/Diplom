using Microsoft.AspNetCore.SignalR;

namespace ChatServer
{
    public class MyHub: Hub
    {
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("hello", "Придумай ник");
            return base.OnConnectedAsync();
        }
    }
}
