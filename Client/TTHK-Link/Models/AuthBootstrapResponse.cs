using System.Text.Json.Serialization;

namespace TTHK_Link.Models;

public class AuthBootstrapResponse
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }  // base64 string from Go []byte

    [JsonPropertyName("user_id")]
    public int ? UserId { get; set; }

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
    public DateTime Created { get; set; }
}

public class BootstrapCourse
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
