using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces;

public interface ISessionCache
{
    void SetBootstrapCourses(List<Course> courses);
    List<Course>? ConsumeBootstrapCourses();
}
