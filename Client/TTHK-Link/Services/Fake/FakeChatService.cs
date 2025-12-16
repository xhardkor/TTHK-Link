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

    public Task<IReadOnlyList<Message>> GetMessagesAsync(string courseId)
    {
        if (!_messages.TryGetValue(courseId, out var list))
        {
            // first
            list = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    CourseId = courseId,
                    UserId = "teacher",
                    Msg = "Tere tulemast chatti!",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    IsMine = false
                }
            };
            _messages[courseId] = list;
        }

        return Task.FromResult<IReadOnlyList<Message>>(list);
    }

    public Task<Message> SendMessageAsync(string courseId, string text, string? imagePath = null)
    {
        var user = _authService.CurrentUser;

        if (!_messages.TryGetValue(courseId, out var list))
        {
            list = new List<Message>();
            _messages[courseId] = list;
        }

        var msg = new Message
        {
            Id = Guid.NewGuid().ToString(),
            CourseId = courseId,
            UserId = user.Id,
            Msg = text,
            ImageUrl = imagePath,
            CreatedAt = DateTime.UtcNow,
            IsMine = true
        };

        list.Add(msg);
        return Task.FromResult(msg);
    }
}
