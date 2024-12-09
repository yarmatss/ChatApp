using ChatApp.API.Data;
using ChatApp.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Hubs;

public class ChatHub : Hub
{
    private readonly DataContext _context;
    private static readonly Dictionary<string, string> _users = new();

    public ChatHub(DataContext context)
    {
        _context = context;
    }

    public async Task JoinChat(string username)
    {
        if (!_users.ContainsKey(Context.ConnectionId))
        {
            _users[Context.ConnectionId] = username;
            await Clients.All.SendAsync("UserJoined", username);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.GetHttpContext()?.Request.Query["username"];
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_users.TryGetValue(Context.ConnectionId, out var username))
        {
            _users.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UserLeft", username);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string user, string message)
    {
        var newMessage = new Message
        {
            User = user,
            Text = message,
            Timestamp = DateTime.UtcNow
        };

        _context.Messages.Add(newMessage);
        await _context.SaveChangesAsync();

        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task<List<Message>> LoadMessages(int pageNumber, int pageSize)
    {
        return await _context.Messages
            .OrderByDescending(m => m.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
