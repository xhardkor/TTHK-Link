using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

public class FakeAuthService : IAuthService
{
    public User? CurrentUser { get; private set; }

    public Task<bool> LoginAsync(LoginRequest request)
    {
        if (request.Username == "admin" &&
            request.Password == "admin")
        {
            CurrentUser = new User
            {
                Id = "1",
                Login = "admin",
                IsAdmin = true,
                GroupId = "TiTge24"
            };

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
