using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json.Serialization;

namespace TTHK_Link.Models;

public class PostMessageApiDto
{
    [JsonPropertyName("room_id")]
    public string RoomId { get; set; } = "";

    [JsonPropertyName("course_id")]
    public int CourseId { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = "";

    [JsonPropertyName("msg")]
    public string Msg { get; set; } = "";

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }
}


