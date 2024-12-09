using ChatApp.Shared;

namespace ChatApp.Client.Services;

public interface IChatService
{
    Task ConnectAsync(string username);
    Task DisconnectAsync();
    Task SendMessageAsync(string user, string message);
    Task<List<Message>> LoadMessagesAsync(int pageNumber, int pageSize);
}
