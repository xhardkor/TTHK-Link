// Services/Fake/FakeChatService.cs
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Fake;

public class FakeChatService : IChatService
{
    private readonly IAuthService _authService;
    private readonly Dictionary<string, List<Message>> _messages = new();

    public FakeChatService(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<IReadOnlyList<Message>> GetMessagesAsync(string groupId)
    {
        if (!_messages.TryGetValue(groupId, out var list))
        {
            // первый фейковый месседж
            list = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groupId,
                    UserId = "teacher",
                    Text = "Tere tulemast chatti!",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    IsMine = false
                }
            };
            _messages[groupId] = list;
        }

        return Task.FromResult<IReadOnlyList<Message>>(list);
    }

    public Task<Message> SendMessageAsync(string groupId, string text, string? imagePath = null)
    {
        var user = _authService.CurrentUser;

        if (!_messages.TryGetValue(groupId, out var list))
        {
            list = new List<Message>();
            _messages[groupId] = list;
        }

        var msg = new Message
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = groupId,
            UserId = user.Id,
            Text = text,
            ImageUrl = imagePath,
            CreatedAt = DateTime.UtcNow,
            IsMine = true
        };

        list.Add(msg);
        return Task.FromResult(msg);
    }
}
