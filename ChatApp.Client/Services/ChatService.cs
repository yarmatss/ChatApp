using ChatApp.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Client.Services;

public class ChatService : IChatService
{
    private readonly HubConnection _hubConnection;

    public event Action<string, string>? OnMessageReceived;
    public event Action<string>? OnUserJoined;
    public event Action<string>? OnUserLeft;

    public ChatService(string baseUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}/chathub")
            .Build();

        _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            OnMessageReceived?.Invoke(user, message));

        _hubConnection.On<string>("UserJoined", user =>
            OnUserJoined?.Invoke(user));

        _hubConnection.On<string>("UserLeft", user =>
            OnUserLeft?.Invoke(user));
    }

    public async Task ConnectAsync(string username)
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("JoinChat", username);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.StopAsync();
        }
    }

    public async Task SendMessageAsync(string user, string message)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("SendMessage", user, message);
        }
    }

    public async Task<List<Message>> LoadMessagesAsync(int pageNumber, int pageSize)
    {
        return await _hubConnection.InvokeAsync<List<Message>>(
            "LoadMessages", pageNumber, pageSize);
    }
}
