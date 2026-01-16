using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using MauiApp = Microsoft.Maui.Controls.Application;


namespace TTHK_Link.ViewModels;

public partial class GroupChatViewModel : ObservableObject
{
    private readonly IChatService _chat;
    private readonly IAuthService _auth;
    private readonly IUserService _users;

    public ObservableCollection<Message> Items { get; } = new();

    [ObservableProperty] private string error = "";
    [ObservableProperty] private string newMessageText = "";
    [ObservableProperty] private bool isBusy;

    public bool CanSendMessage =>
        !string.IsNullOrWhiteSpace(NewMessageText) &&
        _auth.CurrentUser != null;

    public GroupChatViewModel(IChatService chat, IAuthService auth, IUserService users)
    {
        _chat = chat;
        _auth = auth;
        _users = users;
    }

    partial void OnNewMessageTextChanged(string value)
    {
        OnPropertyChanged(nameof(CanSendMessage));
        SendMessageCommand.NotifyCanExecuteChanged();
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

            var allUsers = await _users.GetAllAsync();
            var dict = allUsers.ToDictionary(u => u.Id, u => u.Login);

            var list = await _chat.GetMessagesAsync(roomId);

            foreach (var m in list)
            {
                m.IsMine = (m.UserId == me.Id);
                m.SenderName = dict.TryGetValue(m.UserId, out var name) ? name : "unknown";
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

        var text = NewMessageText.Trim();
        if (text.Length == 0) return;

        NewMessageText = "";

        try
        {
            var sent = await _chat.SendMessageAsync(roomId, me.Id, text);

            // UI fields
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
}
