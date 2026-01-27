using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace TTHK_Link.ViewModels;

public partial class GroupChatViewModel : ObservableObject
{
    private readonly IChatService _chat;
    private readonly IAuthService _auth;
    private DateTime _lastSeen = DateTime.MinValue;
    private readonly HashSet<string> _seenIds = new();


    public ObservableCollection<Message> Items { get; } = new();

    [ObservableProperty] private string error = "";
    [ObservableProperty] private string newMessageText = "";
    [ObservableProperty] private bool isBusy;
    private CancellationTokenSource? _pollCts;
    private bool _polling;


    public bool CanSendMessage =>
        !string.IsNullOrWhiteSpace(NewMessageText) &&
        _auth.CurrentUser != null;

    public GroupChatViewModel(IChatService chat, IAuthService auth)
    {
        _chat = chat;
        _auth = auth;
    }

    partial void OnNewMessageTextChanged(string value)
    {
        OnPropertyChanged(nameof(CanSendMessage));
        SendMessageCommand.NotifyCanExecuteChanged();
    }

    public void StartPolling()
    {
        if (_polling) return;
        _polling = true;

        _pollCts = new CancellationTokenSource();
        var token = _pollCts.Token;

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(2000, token);

                    // ВАЖНО: ObservableCollection обновляем только на UI-потоке
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await RefreshAsync();
                    });
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Polling error: {ex}");
                }
            }
        }, token);
    }

    public void StopPolling()
    {
        _polling = false;
        _pollCts?.Cancel();
        _pollCts = null;
    }


    private string GetRoomId()
    {
        var me = _auth.CurrentUser;
        return me == null ? "" : $"group:{me.GroupId}";
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
            var me = _auth.CurrentUser;
            if (me == null)
            {
                await Shell.Current.GoToAsync("//login");
                return;
            }

            var roomId = GetRoomId();
            if (string.IsNullOrWhiteSpace(roomId))
            {
                Error = "Group missing.";
                return;
            }

            var myId = me.Id?.ToString() ?? "";

            var list = await _chat.GetMessagesAsync(roomId, 0);

            foreach (var m in list)
            {
                var sender = m.UserId?.ToString() ?? "";
                m.IsMine = (sender == myId);

                if (string.IsNullOrWhiteSpace(m.SenderName))
                    m.SenderName = sender;

                Items.Add(m);
            }

            OnPropertyChanged(nameof(CanSendMessage));
            SendMessageCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            Error = "Failed to load group chat.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    public async Task SendMessageAsync()
    {
        var me = _auth.CurrentUser;
        if (me == null) return;

        var roomId = GetRoomId();
        if (string.IsNullOrWhiteSpace(roomId))
        {
            Error = "Group missing.";
            return;
        }

        var text = (NewMessageText ?? "").Trim();
        if (text.Length == 0) return;

        NewMessageText = "";

        try
        {
            var sent = await _chat.SendMessageAsync(roomId, 0, me.Id, text);

            sent.IsMine = true;
            sent.SenderName = me.Login;

            Items.Add(sent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            NewMessageText = text;
            await MauiApp.Current!.MainPage!.DisplayAlert("Error", "Message send failed.", "OK");
        }
    }
    public async Task RefreshAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var me = _auth.CurrentUser;
            if (me == null) return;

            var roomId = GetRoomId();
            if (string.IsNullOrWhiteSpace(roomId)) return;

            var myId = me.Id?.ToString() ?? "";

            var list = await _chat.GetMessagesAsync(roomId, 0);

            foreach (var m in list.OrderBy(x => x.CreatedAt))
            {
                var key = $"{m.CreatedAt:o}|{m.UserId}|{m.Msg}";
                if (_seenIds.Contains(key))
                    continue;

                _seenIds.Add(key);

                var sender = m.UserId?.ToString() ?? "";
                m.IsMine = (sender == myId);

                if (string.IsNullOrWhiteSpace(m.SenderName))
                    m.SenderName = sender;

                Items.Add(m);

                if (m.CreatedAt > _lastSeen)
                    _lastSeen = m.CreatedAt;
            }
        }
        finally
        {
            IsBusy = false;
        }


    }

}
