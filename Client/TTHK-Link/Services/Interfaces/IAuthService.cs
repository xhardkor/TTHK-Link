using TTHK_Link.Models;

public interface IAuthService
{
    User? CurrentUser { get; }

    Task<bool> LoginAsync(LoginRequest request);

    Task LogoutAsync();
}
