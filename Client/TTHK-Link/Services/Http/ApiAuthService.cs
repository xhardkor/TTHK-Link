using System.Linq;
using System.Net;
using System.Text.Json;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Http;

public class ApiAuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ISessionCache _cache;

    public User? CurrentUser { get; private set; }
    public string? Token { get; private set; }

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public ApiAuthService(HttpClient http, ISessionCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
            return false;

        var login = Uri.EscapeDataString(request.Username);
        var pass = Uri.EscapeDataString(request.Password);

        var url = $"{ApiRoute.Login}?login={login}&password={pass}";

        System.Diagnostics.Debug.WriteLine($"LOGIN URL: {url}");

        using var resp = await _http.GetAsync(url);

        var raw = await resp.Content.ReadAsStringAsync();
        System.Diagnostics.Debug.WriteLine("LOGIN RAW JSON:");
        System.Diagnostics.Debug.WriteLine(raw);

        if (resp.StatusCode != HttpStatusCode.OK)
            return false;

        AuthBootstrapResponse? data;
        try
        {
            data = JsonSerializer.Deserialize<AuthBootstrapResponse>(raw, JsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LOGIN PARSE ERROR: {ex}");
            return false;
        }

        //if (data == null || string.IsNullOrWhiteSpace(data.Token))
        //    return false;

        if (data == null)
            return false;

        // token is base64 string (Go []byte)
        Token = string.IsNullOrWhiteSpace(data.Token) ? null : data.Token;

        CurrentUser = new User
        {
            Id = data.User.Id.ToString(),
            Login = string.IsNullOrWhiteSpace(data.User.Username) ? request.Username : data.User.Username,
            GroupId = data.User.GroupId,
            IsAdmin = false
        };

        var courses = data.Courses
            .Select(c => new Course
            {
                Id = c.Id.ToString(),          // <-- id
                GroupId = c.GroupId,
                CourseName = c.Name,
                Description = c.Desc
            })
            .ToList();

        _cache.SetBootstrapCourses(courses);

        System.Diagnostics.Debug.WriteLine($"BOOTSTRAP COURSES COUNT: {courses.Count}");
        System.Diagnostics.Debug.WriteLine($"TOKEN LEN: {(Token == null ? 0 : Token.Length)}");

        return true;


    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.GroupId))
            return false;

        var login = Uri.EscapeDataString(request.Username);
        var pass = Uri.EscapeDataString(request.Password);
        var group = Uri.EscapeDataString(request.GroupId);

        var url =
            $"{ApiRoute.Register}" +
            $"?login={login}" +
            $"&password={pass}" +
            $"&group={group}" +
            $"&group_id={group}";

        System.Diagnostics.Debug.WriteLine($"REGISTER URL: {url}");

        using var resp = await _http.PostAsync(url, content: null);

        var raw = await resp.Content.ReadAsStringAsync();
        System.Diagnostics.Debug.WriteLine($"REGISTER STATUS: {(int)resp.StatusCode} {resp.ReasonPhrase}");
        System.Diagnostics.Debug.WriteLine($"REGISTER RAW RESPONSE: {raw}");

        if (resp.StatusCode != HttpStatusCode.OK &&
            resp.StatusCode != HttpStatusCode.Created)
            return false;

        TokenResponse? tokenData;
        try
        {
            tokenData = JsonSerializer.Deserialize<TokenResponse>(raw, JsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"REGISTER PARSE ERROR: {ex}");
            return false;
        }

        if (tokenData == null || string.IsNullOrWhiteSpace(tokenData.Token))
            return false;

        Token = tokenData.Token;

        CurrentUser = new User
        {
            Id = "0",
            Login = request.Username,
            IsAdmin = false,
            GroupId = request.GroupId
        };

        return true;
    }

    public Task LogoutAsync()
    {
        CurrentUser = null;
        Token = null;
        return Task.CompletedTask;
    }
}
