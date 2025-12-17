using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface ICourseService
{
    Task<List<Course>> GetCoursesForUserAsync(User user);
}
