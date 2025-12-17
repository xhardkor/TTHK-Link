using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

public class FakeCourseService : ICourseService
{
    public Task<List<Course>> GetCoursesForUserAsync(User user)
    {
        var allCourses = new List<Course>
        {
            new Course
            {
                Id = "c1",
                GroupId = "TiTge24",
                CourseName = "Programmeerimise alused",
                Description = "Tava vchat"
            },
            new Course
            {
                Id = "c2",
                GroupId = "TiTge24",
                CourseName = "Võrgud",
                Description = "Materjalid ja arutelud"
            },
            new Course
            {
                Id = "c3",
                GroupId = "TiTge24",
                CourseName = "Andmebaasid",
                Description = "Teine rühm"
            }
        };

        // Filtreerime: kasutaja näeb ainult oma rühma kursuseid
        var visibleCourses = allCourses
            .Where(c => c.GroupId == user.GroupId)
            .ToList();

        return Task.FromResult(visibleCourses);

    
    }
}
