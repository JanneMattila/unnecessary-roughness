using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using UR.Events;

namespace UR.Server.Hubs
{
    public class GameHub : Hub
    {
        public async Task AppendEvent(Event e)
        {
            await Clients.All.SendAsync(HubConstants.AppendEventMethod, e);
        }
    }
}
