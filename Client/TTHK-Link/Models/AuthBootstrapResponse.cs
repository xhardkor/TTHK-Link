using System.Text.Json.Serialization;

namespace TTHK_Link.Models;

public class AuthBootstrapResponse
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("user")]
    public BootstrapUser User { get; set; } = new();

    [JsonPropertyName("courses")]
    public List<BootstrapCourse> Courses { get; set; } = new();
}

public class BootstrapUser
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("user")]
    public string Username { get; set; } = "";

    [JsonPropertyName("group_id")]
    public string GroupId { get; set; } = "";

    [JsonPropertyName("created")]
    public string Created { get; set; } = "";
}

public class BootstrapCourse
{
    [JsonPropertyName("group_id")]
    public string GroupId { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = "";
}
