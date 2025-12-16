using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Fake;

public class FakeChatService : IChatService
{
    private readonly List<Message> _all = new()
    {
        new Message { Id="m1", CourseId="c1", UserId="1", Msg="Tere!", CreatedAt=DateTime.Now.AddMinutes(-10) },
        new Message { Id="m2", CourseId="c1", UserId="2", Msg="Tsau!", CreatedAt=DateTime.Now.AddMinutes(-8) },
        new Message { Id="m3", CourseId="c2", UserId="1", Msg="Mis toimub?", CreatedAt=DateTime.Now.AddMinutes(-5) },
    };

    public Task<List<Message>> GetMessagesAsync(string courseId)
    {
        // Tagastame ainult valitud kursuse sõnumid
        var list = _all.Where(m => m.CourseId == courseId).ToList();
        return Task.FromResult(list);
    }
}
