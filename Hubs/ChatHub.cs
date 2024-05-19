using Microsoft.AspNetCore.SignalR;
namespace PROJECTALTERAPI.Hubs;

public sealed class ChatHub : Hub
{
    public async Task SendMessage(long senderId, long receiverId, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", senderId, receiverId, message);
    }



























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
