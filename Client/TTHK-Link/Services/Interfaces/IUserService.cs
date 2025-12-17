using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string userId);
}
