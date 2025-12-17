namespace TTHK_Link.Models
{
    public class AuthResult
    {
        public User User { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
