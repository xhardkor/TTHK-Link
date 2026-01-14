using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services;

public class SessionCache : ISessionCache
{
    private List<Course>? _courses;

    public void SetBootstrapCourses(List<Course> courses)
    {
        _courses = courses ?? new List<Course>();
    }

    public List<Course>? ConsumeBootstrapCourses()
    {
        var tmp = _courses;
        _courses = null;
        return tmp;
    }
}
