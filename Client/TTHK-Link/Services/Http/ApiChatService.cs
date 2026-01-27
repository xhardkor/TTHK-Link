using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;
using System.Text.Json;

namespace TTHK_Link.Services.Http;

public class ApiChatService : IChatService
{
    private readonly HttpClient _http;
    private readonly IAuthService _auth;

    private static readonly JsonSerializerOptions JsonOptions =
     new(JsonSerializerDefaults.Web)
     {
         PropertyNameCaseInsensitive = true
     };

    public ApiChatService(HttpClient http, IAuthService auth)
    {
        _http = http;
        _auth = auth;
    }

  
    private string? TryGetToken() => (_auth as ApiAuthService)?.Token;

    private void ApplyAuthHeader()
    {
        var token = TryGetToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization = null;
            return;
        }

        //  Bearer title
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    // roomId format: "{roomKey}/{holderId}"
    private static (string roomKey, int holderId) ParseRoomId(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            return ("", 0);

        var parts = roomId.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return (roomId.Trim(), 0);

        var roomKey = parts[0].Trim();
        if (!int.TryParse(parts[1], out var holderId))
            holderId = 0;

        return (roomKey, holderId);
    }

    public async Task<Message> SendMessageAsync(string roomId, int courseId, string userId, string msg)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new ArgumentException("roomId is empty");

        msg = (msg ?? "").Trim();
        if (msg.Length == 0)
            throw new ArgumentException("msg is empty");

        ApplyAuthHeader();

        var payload = new PostMessageApiDto
        {
            RoomId = roomId,
            CourseId = courseId,
            UserId = userId,
            Msg = msg,
            Created = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var resp = await _http.PostAsync(ApiRoute.PostMessage, content);
        var raw = await resp.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"POST STATUS: {(int)resp.StatusCode}");
        System.Diagnostics.Debug.WriteLine($"POST RAW: {raw}");

        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException("Send failed");

        return new Message
        {
            Id = Guid.NewGuid().ToString(),
            CourseId = courseId.ToString(),
            UserId = userId,
            Msg = msg,
            CreatedAt = DateTime.Now,
            SenderName = userId,
            IsMine = false,
            ImageUrl = null
        };
    }


    public async Task<List<Message>> GetMessagesAsync(string roomId, int courseId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            return new();

        ApplyAuthHeader();

        var url = $"{ApiRoute.GetMessages}/{Uri.EscapeDataString(roomId)}/{courseId}";
        System.Diagnostics.Debug.WriteLine($"GET URL: {url}");

        using var resp = await _http.GetAsync(url);
        var raw = await resp.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"GET STATUS: {(int)resp.StatusCode}");
        System.Diagnostics.Debug.WriteLine($"GET RAW: {raw}");

        if (!resp.IsSuccessStatusCode)
            return new();

        List<MessageApiDto>? data;
        try
        {
            data = JsonSerializer.Deserialize<List<MessageApiDto>>(raw, JsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GET PARSE ERROR: {ex}");
            return new();
        }

        if (data == null) return new();

        return data.Select(x => new Message
        {
            Id = (x.Id ?? 0).ToString(),
            CourseId = courseId.ToString(),
            UserId = x.User.Username,
            Msg = x.Msg,
            CreatedAt = x.Created,
            SenderName = x.User.Username,
            IsMine = false,
            ImageUrl = null
        })
          .OrderBy(m => m.CreatedAt)
          .ToList();
    }



}
