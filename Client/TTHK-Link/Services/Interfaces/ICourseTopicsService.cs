using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface ICourseTopicsService
{
    Task<List<CourseTopic>> GetTopicsAsync(string courseId);
    Task<CourseTopic> CreateTopicAsync(string courseId, string title, string body, User author);
}
