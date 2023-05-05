using Microsoft.AspNetCore.SignalR;

namespace Forfeit15.Preferences.Core.Services;

public class UpdateHub : Hub
{
    private readonly IHubContext<UpdateHub> _hubContext;

    public UpdateHub(IHubContext<UpdateHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client {Context.ConnectionId} connected");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Client {Context.ConnectionId} disconnected");
    }
    
    public async Task SendUpdate(string[] clientIds, string message, CancellationToken cancellationToken)
    {
        await Clients.All.SendAsync("ReceiveMessage", message, cancellationToken);
        Console.WriteLine($"No active clients for user ids {string.Join(",", clientIds)}");
    }
}