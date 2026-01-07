namespace TTHK_Link.Models;

public class ChatThread
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string? LastMessage { get; set; }
    public DateTime? LastAt { get; set; }
    public int UnreadCount { get; set; }
}