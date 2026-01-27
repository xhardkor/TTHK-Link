using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(CourseName), "courseName")]
public partial class CourseTopicsViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly IChatService _chat;

    [ObservableProperty] private string courseId = "";
    [ObservableProperty] private string courseName = "";
    [ObservableProperty] private string error = "";
    [ObservableProperty] private bool isBusy;

    public ObservableCollection<CourseTopic> Items { get; } = new();

    public CourseTopicsViewModel(IAuthService auth, IChatService chat)
    {
        _auth = auth;
        _chat = chat;
    }

    private string GetRoomId()
    {
        return string.IsNullOrWhiteSpace(CourseId) ? "" : $"course:{CourseId}";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        Error = "";
        Items.Clear();

        try
        {
            if (string.IsNullOrWhiteSpace(CourseId))
            {
                Error = "CourseId missing.";
                return;
            }

            var roomId = GetRoomId();
            if (string.IsNullOrWhiteSpace(roomId))
            {
                Error = "RoomId missing.";
                return;
            }

            var list = await _chat.GetMessagesAsync(roomId, 0);

            foreach (var m in list)
            {
                Items.Add(new CourseTopic
                {
                    Id = m.Id,
                    CourseId = CourseId,
                    Title = ExtractTitle(m.Msg),
                    Body = ExtractBody(m.Msg),
                    AuthorLogin = m.SenderName,
                    CreatedAt = m.CreatedAt
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            Error = "Failed to load topics.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task AddTopicAsync()
    {
        Error = "";

        var user = _auth.CurrentUser;
        if (user == null)
        {
            await Shell.Current.GoToAsync("//login");
            return;
        }

        if (string.IsNullOrWhiteSpace(CourseId))
        {
            Error = "CourseId missing.";
            return;
        }

        var title = await Shell.Current.DisplayPromptAsync("New topic", "Title:", "Create", "Cancel");
        if (string.IsNullOrWhiteSpace(title))
            return;

        var body = await Shell.Current.DisplayPromptAsync("New topic", "Message:", "Create", "Cancel");
        if (string.IsNullOrWhiteSpace(body))
            return;

        var roomId = GetRoomId();

        var msg = $"{title.Trim()}\n{body.Trim()}";
        await _chat.SendMessageAsync(roomId, 0, user.Id, msg);

        await LoadAsync();
    }

    [RelayCommand]
    public async Task OpenTopicAsync(CourseTopic topic)
    {
        if (topic == null) return;

        var topicId = Uri.EscapeDataString(topic.Id);
        var courseId = Uri.EscapeDataString(CourseId);

        await Shell.Current.GoToAsync($"topicChat?courseId={courseId}&topicId={topicId}");
    }

    private static string ExtractTitle(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg)) return "";
        var idx = msg.IndexOf('\n');
        return idx >= 0 ? msg[..idx].Trim() : msg.Trim();
    }

    private static string ExtractBody(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg)) return "";
        var idx = msg.IndexOf('\n');
        return idx >= 0 ? msg[(idx + 1)..].Trim() : "";
    }
}
