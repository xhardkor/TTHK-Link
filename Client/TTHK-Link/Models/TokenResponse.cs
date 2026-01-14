using System.Text.Json.Serialization;

namespace TTHK_Link.Models;

public class TokenResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = "";
}