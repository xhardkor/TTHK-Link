using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Fake;

public class FakeCourseTopicsService //: ICourseTopicsService
{
    private readonly List<CourseTopic> _topics = new();

    public FakeCourseTopicsService()
    {
        _topics.Add(new CourseTopic
        {
            Id = "t1",
            CourseId = "c1",
            Title = "Kodutoo nr. 5",
            Body = "Kuidas lahendada see probleem?",
            AuthorLogin = "admin",
            CreatedAt = DateTime.UtcNow.AddHours(-5)
        });
    }

    public Task<List<CourseTopic>> GetTopicsAsync(string courseId)
    {
        var list = _topics
            .Where(t => t.CourseId == courseId)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        return Task.FromResult(list);
    }

    public Task<CourseTopic> CreateTopicAsync(string courseId, string title, string body, User author)
    {
        var t = new CourseTopic
        {
            Id = Guid.NewGuid().ToString("N"),
            CourseId = courseId,
            Title = title,
            Body = body,
            AuthorLogin = author.Login,
            CreatedAt = DateTime.UtcNow
        };

        _topics.Add(t);
        return Task.FromResult(t);
    }
}
