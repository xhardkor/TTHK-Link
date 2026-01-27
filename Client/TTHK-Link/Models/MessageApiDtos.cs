using System.Text.Json.Serialization;

namespace TTHK_Link.Models;

public class MessageApiDto
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; } = "";

    [JsonPropertyName("room_id")]
    public string RoomId { get; set; } = "";

    [JsonPropertyName("user")]
    public MessageUserApiDto User { get; set; } = new();
}

public class MessageUserApiDto
{
    [JsonPropertyName("user")]
    public string Username { get; set; } = "";
}
