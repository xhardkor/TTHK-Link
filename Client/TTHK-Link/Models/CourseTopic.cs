namespace TTHK_Link.Models;

public class CourseTopic
{
    public string Id { get; set; } = "";
    public string CourseId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string AuthorLogin { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
