namespace TTHK_Link.Models;

public class AuthLoginResponse
{
    public string Token { get; set; } = "";
    public User User { get; set; } = new();
}
