namespace TTHK_Link.Services.Http;

internal static class ApiRoute
{
    public const string Register = "/auth/create";
    public const string Login = "/auth";
    public const string UserInfo = "/user/info";

    //chat
    public const string GetMessages = "/messages"; // + /{room_id}/{holder_id}
    public const string PostMessage = "/message";  // POST

    public const string GetUserCourses = "/user/courses";
}
//