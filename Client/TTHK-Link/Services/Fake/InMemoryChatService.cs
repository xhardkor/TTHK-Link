//using TTHK_Link.Models;
//using TTHK_Link.Services.Interfaces;

//namespace TTHK_Link.Services.Fake;

//public class InMemoryChatService : IChatService
//{
//    private readonly List<Message> _all = new()
//    {
//        new Message { Id="m1", CourseId="group:TITge24", UserId="1", Msg="Tere!", CreatedAt=DateTime.Now.AddMinutes(-10) },
//        new Message { Id="m2", CourseId="group:TITge24", UserId="2", Msg="Tsau!", CreatedAt=DateTime.Now.AddMinutes(-8) },
//    };

//    public Task<List<Message>> GetMessagesAsync(string roomId)
//        => Task.FromResult(
//            _all
//                .Where(m => m.CourseId == roomId)
//                .OrderBy(m => m.CreatedAt)
//                .ToList()
//        );

//    public Task<Message> SendMessageAsync(string roomId, int userId, string msg)
//    {
//        var m = new Message
//        {
//            Id = $"m{_all.Count + 1}",
//            CourseId = roomId,
//            UserId = userId.ToString(),
//            Msg = msg,
//            CreatedAt = DateTime.Now
//        };

//        _all.Add(m);
//        return Task.FromResult(m);
//    }
//}
