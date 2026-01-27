using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Fake;

public class FakeTopicCommentsService : ITopicCommentsService
{
    private readonly List<TopicComment> _comments = new();

    public FakeTopicCommentsService()
    {
        _comments.Add(new TopicComment
        {
            Id = "cm1",
            TopicId = "t1",
            UserId = "admin",
            Text = "juba ise tegin thanks.",
            CreatedAt = DateTime.UtcNow.AddMinutes(-40)
        });
    }

    public Task<List<TopicComment>> GetCommentsAsync(string topicId)
    {
        var list = _comments
            .Where(c => c.TopicId == topicId)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        return Task.FromResult(list);
    }

    public Task<TopicComment> AddCommentAsync(string topicId, string text, User author)
    {
        var c = new TopicComment
        {
            Id = Guid.NewGuid().ToString("N"),
            TopicId = topicId,
            Text = text,
            UserId = author.Id, // Fixed: use UserId instead of AuthorLogin
            CreatedAt = DateTime.UtcNow
        };

        _comments.Add(c);
        return Task.FromResult(c);
    }
}
