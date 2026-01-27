using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface IChatService
{
    Task<List<Message>> GetMessagesAsync(string roomId, int courseId);
    Task<Message> SendMessageAsync(string roomId, int courseId, string userId, string msg);
}
