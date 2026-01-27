using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace TTHK_Link.ViewModels;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(TopicId), "topicId")]
public partial class TopicChatViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly IChatService _chat;

    [ObservableProperty] private string courseId = "";
    [ObservableProperty] private string topicId = "";
    [ObservableProperty] private string error = "";
    [ObservableProperty] private string newMessage = "";
    [ObservableProperty] private bool isBusy;

    public ObservableCollection<TopicComment> Items { get; } = new();

    public TopicChatViewModel(IAuthService auth, IChatService chat)
    {
        _auth = auth;
        _chat = chat;
    }

    private string GetRoomId()
    {
        return string.IsNullOrWhiteSpace(CourseId) ? "" : $"course:{CourseId}";
    }

    private bool TryGetTopicInt(out int tid)
    {
        return int.TryParse(TopicId, out tid) && tid > 0;
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

            if (string.IsNullOrWhiteSpace(TopicId) || !TryGetTopicInt(out var tid))
            {
                Error = "TopicId invalid.";
                return;
            }

            var roomId = GetRoomId();
            if (string.IsNullOrWhiteSpace(roomId))
            {
                Error = "RoomId missing.";
                return;
            }

            var msgs = await _chat.GetMessagesAsync(roomId, tid);

            foreach (var m in msgs)
            {
                Items.Add(new TopicComment
                {
                    Id = m.Id,
                    TopicId = TopicId,
                    UserId = m.UserId,
                    AuthorLogin = m.SenderName, 
                    Text = m.Msg,
                    CreatedAt = m.CreatedAt
                });

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            Error = "Failed to load comments.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SendAsync()
    {
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

        if (string.IsNullOrWhiteSpace(TopicId) || !TryGetTopicInt(out var tid))
        {
            Error = "TopicId invalid.";
            return;
        }

        var text = (NewMessage ?? "").Trim();
        if (text.Length == 0) return;

        try
        {
            var roomId = GetRoomId();
            await _chat.SendMessageAsync(roomId, tid, user.Id, text);
            NewMessage = "";
            await LoadAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await MauiApp.Current!.MainPage!.DisplayAlert("Error", "Comment send failed.", "OK");
        }
    }
}
