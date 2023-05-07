using Microsoft.AspNetCore.SignalR;

namespace Forfeit15.Preferences.Core.Services;

public class UpdateHub : Hub
{
    private readonly IHubContext<UpdateHub> _hubContext;
    private static Dictionary<string, string> userConnectionMap = new Dictionary<string, string>();

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
    
    /// <summary>
    /// Associates a userId to the connection Id and stores it in a temp dictionary.
    /// </summary>
    /// <param name="userId"></param>
    public async Task Connect(string userId)
    {
        userConnectionMap[userId] = Context.ConnectionId;
        Console.WriteLine(userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
    
    /// <summary>
    /// Sends a message to connected users based on a 'preference'.
    /// </summary>
    /// <param name="clientIds"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    public async Task SendMessageAsync(string[] clientIds, string message, CancellationToken cancellationToken)
    {
        foreach (var clientId in clientIds)
        {
            if (userConnectionMap.TryGetValue(clientId, out var connectionId))
            {
                await Clients.Clients(connectionId).SendAsync("ReceiveMessage", message, cancellationToken);
            }
            else
            {
                Console.WriteLine($"No active clients for user ids {string.Join(",", clientIds)}");
            }
        }
    }
}