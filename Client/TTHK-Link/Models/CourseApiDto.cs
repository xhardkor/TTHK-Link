using System.Text.Json.Serialization;

namespace TTHK_Link.Models;

public class CourseApiDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("group_id")]
    public string GroupId { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = "";
}
