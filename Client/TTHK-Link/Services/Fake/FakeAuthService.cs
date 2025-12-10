using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Fake;

public class FakeAuthService : IAuthService
{
    private User? _currentUser;

    public User CurrentUser
    {
        get
        {
            if (_currentUser == null)
                throw new InvalidOperationException("User is not logged in");

            return _currentUser;
        }
    }

    public Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = request.Username,
            IsAdmin = request.Username == "admin",
            GroupNameId = "TiTge24"
        };

        _currentUser = user;

        return Task.FromResult(new AuthResult
        {
            User = user,
            Token = "fake-token"
        });
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        return Task.CompletedTask;
    }
}
