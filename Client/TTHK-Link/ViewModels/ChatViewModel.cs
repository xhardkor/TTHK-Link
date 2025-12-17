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

    // Uus: sisestatav sõnum
    [ObservableProperty] private string newMessageText = "";

    // Uus: nupu aktiivsuse kontroll UI jaoks
    public bool CanSendMessage =>
        !string.IsNullOrWhiteSpace(NewMessageText) &&
        !string.IsNullOrWhiteSpace(CourseId) &&
        _auth.CurrentUser != null;

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

        // Uuendame ka nupu staatust
        OnPropertyChanged(nameof(CanSendMessage));
        SendMessageCommand.NotifyCanExecuteChanged();
    }

    partial void OnNewMessageTextChanged(string value)
    {
        // Kui tekst muutub, uuendame nupu aktiivsust
        OnPropertyChanged(nameof(CanSendMessage));
        SendMessageCommand.NotifyCanExecuteChanged();
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

    // Uus: sõnumi saatmine
    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    public async Task SendMessageAsync()
    {
        var me = _auth.CurrentUser;
        if (me == null) return;
        if (string.IsNullOrWhiteSpace(CourseId)) return;

        var text = NewMessageText.Trim();
        if (text.Length == 0) return;

        // Tühjendame sisestuse kohe (UI tundub kiirem)
        NewMessageText = "";

        // NB! Siin sõltub signatuur sinu IChatService'ist.
        // Kui sul on juba SendMessageAsync, kasuta seda.
        // Kui veel pole, siis lisame selle järgmise sammuna.
        try
        {
            // Oletus: teenus tagastab loodud Message
            var sent = await _chat.SendMessageAsync(
                CourseId,
                me.Id.ToString(),
                text
            );

            // Kui teenus tagastab null või sul pole veel serverit, teeme lokaalse lisamise
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
            // Kui saatmine ebaõnnestub, paneme teksti tagasi (et kasutaja ei kaotaks seda)
            NewMessageText = text;

            await Application.Current!.MainPage!.DisplayAlert(
                "Viga",
                "Sõnumi saatmine ebaõnnestus.",
                "OK");
        }
    }
}
