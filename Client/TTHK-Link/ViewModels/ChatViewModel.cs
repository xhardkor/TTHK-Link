using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

// Võtame courseId Shell query'st: chat?courseId=...
[QueryProperty(nameof(CourseId), "courseId")]
public partial class ChatViewModel : ObservableObject
{
    private readonly IChatService _chat;
    private readonly IAuthService _auth;
    private readonly IUserService _users;

    public ObservableCollection<Message> Items { get; } = new();

    [ObservableProperty] private string courseId = "";

    public ChatViewModel(IChatService chat, IAuthService auth, IUserService users)
    {
        _chat = chat;
        _auth = auth;
        _users = users;
    }

    partial void OnCourseIdChanged(string value)
    {
        // Kui courseId muutub, laeme sõnumid uuesti
        _ = LoadAsync();
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        Items.Clear();  

        var me = _auth.CurrentUser;
        if (me == null || string.IsNullOrWhiteSpace(CourseId))
            return;

        var allUsers = await _users.GetAllAsync();

        var dict = allUsers.ToDictionary(u => u.Id, u => u.Login);

        // Laeme ainult selle kursuse sõnumid
        var list = await _chat.GetMessagesAsync(CourseId);

        foreach (var m in list)
        {
            // UI helper: kas see sõnum on minu oma
            m.IsMine = (m.UserId == me.Id);

            // UI jaoks: saatja nimi
            m.SenderName = dict.TryGetValue(m.UserId, out var name)
                ? name
                : "unknown";

            Items.Add(m);
        }
    }
}
