using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Fake;

public class FakeUserService : IUserService
{
    // Fake kasutajad (nagu tuleksid DB-st)
    private readonly List<User> _users = new()
    {
        new User { Id = "1", Login = "admin", GroupId = "TiTge24", IsAdmin = true },
        new User { Id = "2", Login = "mari",  GroupId = "TiTge24", IsAdmin = false },
        new User { Id = "3", Login = "jaan",  GroupId = "TiTge24", IsAdmin = false },
        new User { Id = "4", Login = "anna",  GroupId = "TiTge23", IsAdmin = false }
    };

    public Task<List<User>> GetAllAsync()
        => Task.FromResult(_users);

    public Task<User?> GetByIdAsync(string userId)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
}
