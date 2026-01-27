using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface ITopicCommentsService
{
    Task<List<TopicComment>> GetCommentsAsync(string topicId);
    Task<TopicComment> AddCommentAsync(string topicId, string text, User author);
}
