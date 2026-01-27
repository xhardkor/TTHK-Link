using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.Services.Http
{
    public class ApiCourseService : ICourseService
    {
        private readonly HttpClient _http;
        private readonly IAuthService _auth;

        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

        public ApiCourseService(HttpClient http, IAuthService auth)
        {
            _http = http;
            _auth = auth;
        }

        private string? TryGetToken() => (_auth as ApiAuthService)?.Token;

        private void ApplyAuthHeader()
        {
            var token = TryGetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                _http.DefaultRequestHeaders.Authorization = null;
                return;
            }

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<Course>> GetCoursesForUserAsync(User user)
        {
            ApplyAuthHeader(); 

            var userId = Uri.EscapeDataString(user.Id); // "5"
            var url = $"{ApiRoute.GetUserCourses}?id={userId}";

            System.Diagnostics.Debug.WriteLine($"COURSES URL: {url}");

            using var resp = await _http.GetAsync(url);
            var raw = await resp.Content.ReadAsStringAsync();

            if (resp.StatusCode != HttpStatusCode.OK)
            {
                System.Diagnostics.Debug.WriteLine($"COURSES ERROR {(int)resp.StatusCode}: {raw}");
                return new();
            }

            List<CourseApiDto>? data;
            try
            {
                data = JsonSerializer.Deserialize<List<CourseApiDto>>(raw, JsonOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"COURSES PARSE ERROR: {ex}");
                return new();
            }

            if (data == null) return new();

            return data.Select(c => new Course
            {
                Id = c.Id.ToString(),
                GroupId = c.GroupId,
                CourseName = c.Name,
                Description = c.Desc
            }).ToList();
        }

    }
}
