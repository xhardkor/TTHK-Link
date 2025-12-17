using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface IChatService
{
    Task<List<Message>> GetMessagesAsync(string courseId);

    // Uus: sõnumi saatmine
    Task<Message> SendMessageAsync(string courseId, string userId, string msg);
}
