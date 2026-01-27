using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace TTHK_Link.Models;

public class CourseTopicApiDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("course_id")]
    public string CourseId { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("body")]
    public string Body { get; set; } = "";

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("author")]
    public TopicAuthorApiDto Author { get; set; } = new();
}

public class TopicAuthorApiDto
{
    [JsonPropertyName("user")]
    public string Username { get; set; } = "";
}
