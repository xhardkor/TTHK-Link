using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

// Берём courseId из Shell-навигации: chat?courseId=...
// ReSharper disable once UnusedMember.Global
[QueryProperty(nameof(CourseId), queryId: "courseId")]
public partial class ChatViewModel : ObservableObject
{
    private readonly IChatService _chat;
    private readonly IAuthService _auth;
    private readonly IUserService _users;

    // Список сообщений для UI
    public ObservableCollection<Message> Items { get; } = new();

    // courseId из навигации
    // ReSharper disable once UnusedMember.Local
    [ObservableProperty]
    private string courseId = "";

    // Текст вводимого сообщения
    // ReSharper disable once UnusedMember.Local
    [ObservableProperty]
    private string newMessageText = "";

    // Можно ли отправлять сообщение (UX)
    public bool CanSendMessage =>
        !string.IsNullOrWhiteSpace(NewMessageText) &&
        !string.IsNullOrWhiteSpace(CourseId) &&
        _auth.CurrentUser != null;

    public ChatViewModel(
        IChatService chat,
        IAuthService auth,
        IUserService users)
    {
        _chat = chat;
        _auth = auth;
        _users = users;
    }

    // Когда меняется courseId — загружаем сообщения
    partial void OnCourseIdChanged(string value)
    {
        _ = LoadAsync();
        OnPropertyChanged(nameof(CanSendMessage));
        SendMessageCommand.NotifyCanExecuteChanged();
    }

    // Когда меняется текст — обновляем состояние кнопки
    partial void OnNewMessageTextChanged(string value)
    {
        OnPropertyChanged(nameof(CanSendMessage));
        SendMessageCommand.NotifyCanExecuteChanged();
    }

    // Загрузка сообщений
    [RelayCommand]
    public async Task LoadAsync()
    {
        Items.Clear();

        var me = _auth.CurrentUser;
        if (me == null || string.IsNullOrWhiteSpace(CourseId))
            return;

        var allUsers = await _users.GetAllAsync();
        var dict = allUsers.ToDictionary(u => u.Id, u => u.Login);

        var list = await _chat.GetMessagesAsync(CourseId);

        foreach (var m in list)
        {
            m.IsMine = m.UserId == me.Id;
            m.SenderName = dict.TryGetValue(m.UserId, out var name)
                ? name
                : "unknown";

            Items.Add(m);
        }
    }

    // Отправка сообщения
    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    public async Task SendMessageAsync()
    {
        var me = _auth.CurrentUser;
        if (me == null || string.IsNullOrWhiteSpace(CourseId))
            return;

        var text = NewMessageText.Trim();
        if (text.Length == 0)
            return;

        // Очищаем поле сразу (UX)
        NewMessageText = "";

        try
        {
            var sent = await _chat.SendMessageAsync(
                CourseId,
                me.Id.ToString(),
                text
            );

            // Если сервер не вернул сообщение — создаём локально
            if (sent == null)
            {
                sent = new Message
                {
                    UserId = me.Id,
                    Msg = text,
                    CreatedAt = DateTime.Now
                };
            }

            sent.IsMine = true;
            sent.SenderName = me.Login;

            Items.Add(sent);
        }
        catch
        {
            // Возвращаем текст, если отправка не удалась
            NewMessageText = text;

            await Shell.Current.DisplayAlert("Ошибка", "Не удалось отправить сообщение.", "OK");
        }
    }
}