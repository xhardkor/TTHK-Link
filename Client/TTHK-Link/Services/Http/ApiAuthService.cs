using System.Net;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Http;

public class ApiAuthService : IAuthService
{
    private readonly HttpClient _http;

    public User? CurrentUser { get; private set; }

    public ApiAuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.GroupId))
            return false;

        var url = BuildUrl("/auth", new Dictionary<string, string>
        {
            ["login"] = request.Username,
            ["password"] = request.Password,
            ["groupid"] = request.GroupId
        });

        using var resp = await _http.PostAsync(url, content: null);

        if (resp.StatusCode == HttpStatusCode.OK)
        {
            CurrentUser = new User
            {
                Id = "0",
                Login = request.Username,
                IsAdmin = false,
                GroupId = request.GroupId
            };
            return true;
        }

        return false;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        // NB! Hiljem teeme päris login/tokeni.
        return await Task.FromResult(false);
    }

    public Task LogoutAsync()
    {
        CurrentUser = null;
        return Task.CompletedTask;
    }

    private static string BuildUrl(string path, Dictionary<string, string> query)
    {
        var qp = string.Join("&", query.Select(kv =>
            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

        return $"{path}?{qp}";
    }
}
