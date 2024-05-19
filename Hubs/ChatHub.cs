using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
namespace PROJECTALTERAPI.Hubs;

public sealed class ChatHub : Hub
{
    private static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier; // Get the user ID from the current context
        if (userId != null)
        {
            _connections.TryAdd(Context.ConnectionId, userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.TryRemove(Context.ConnectionId, out _);

        await base.OnDisconnectedAsync(exception);
    }
    public async Task SendMessage(long senderId, long receiverId, string message)
    {
        var receiverConnectionId = _connections.FirstOrDefault(x => x.Value == receiverId.ToString()).Key;
        if (receiverConnectionId != null)
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, receiverId, message);
        }
    }
    /*     private static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();
     */
    /*     public async Task SendMessage(long senderId, long receiverId, string message)
        {
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, receiverId, message);
        } */

    /*     public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Receive message", $"{Context.ConnectionId} has joined the chat");
        } */
    /* private readonly Sharedb _shared;

    public ChatHub(Sharedb shared)
    {
        _shared = shared ?? throw new ArgumentNullException(nameof(shared));
    }
    public async Task JoinSpecificGroup(UserConnection conn)
    {
        try
        {
            // Add the connection to the specified chat room group
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);

            // Store the connection in the shared database
            _shared.Connections[Context.ConnectionId] = conn;

            // Send a message to the chat room group indicating that the user has joined
            await Clients.Group(conn.ChatRoom).SendAsync("ReciveMessage", conn.Username, $"{conn.Username} has joined {conn.ChatRoom}");
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur
            Console.WriteLine($"An error occurred in JoinSpecificGroup: {ex.Message}");
            throw; // Re-throw the exception to indicate that an error occurred
        }
    }

    public async Task SendMessage(string msg)
    {
        try
        {
            // Retrieve the user connection associated with the current connection ID
            if (_shared.Connections.TryGetValue(Context.ConnectionId, out UserConnection? conn) && conn != null)
            {
                // Send the message to the chat room group
                await Clients.Groups(conn.ChatRoom).SendAsync("SendMessage", conn.Username, msg);
            }
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur
            Console.WriteLine($"An error occurred in SendMessage: {ex.Message}");
            throw; // Re-throw the exception to indicate that an error occurred
        }
    } */
}
