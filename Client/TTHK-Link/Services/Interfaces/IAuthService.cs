using TTHK_Link.Models;

public interface IAuthService
{
    User? CurrentUser { get; }

    Task<bool> LoginAsync(LoginRequest request);

    // Uus: kasutaja registreerimine
    Task<bool> RegisterAsync(LoginRequest request);

    Task LogoutAsync();
}
