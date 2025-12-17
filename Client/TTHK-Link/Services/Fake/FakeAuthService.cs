using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

public class FakeAuthService : IAuthService
{
    public User? CurrentUser { get; private set; }

    // Lihtne "andmebaas" mälus
    private readonly Dictionary<string, (string Password, User User)> _users = new();

    public FakeAuthService()
    {
        // Vaikimisi kasutaja
        var adminUser = new User
        {
            Id = "1",
            Login = "admin",
            IsAdmin = true,
            GroupId = "TiTge24"
        };

        _users["admin"] = ("admin", adminUser);
    }

    public Task<bool> RegisterAsync(LoginRequest request)
    {
        // Kontrollime sisendit
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult(false);
        }

        // Kui kasutaja juba olemas, siis ei registreeri
        if (_users.ContainsKey(request.Username))
            return Task.FromResult(false);

        var newUser = new User
        {
            Id = (_users.Count + 1).ToString(),
            Login = request.Username,
            IsAdmin = false,
            GroupId = "TiTge24"
        };

        _users[request.Username] = (request.Password, newUser);
        CurrentUser = newUser;

        return Task.FromResult(true);
    }

    public Task<bool> LoginAsync(LoginRequest request)
    {
        if (_users.TryGetValue(request.Username, out var entry) &&
            entry.Password == request.Password)
        {
            CurrentUser = entry.User;
            return Task.FromResult(true);
        }

        CurrentUser = null;
        return Task.FromResult(false);
    }


    public Task LogoutAsync()
    {
        CurrentUser = null;
        return Task.CompletedTask;
    }
}
