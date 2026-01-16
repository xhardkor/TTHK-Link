using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface IChatService
{
    Task<List<Message>> GetMessagesAsync(string roomId);
    Task<Message> SendMessageAsync(string roomId, string userId, string msg);

}