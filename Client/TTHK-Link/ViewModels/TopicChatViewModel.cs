using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

[QueryProperty(nameof(TopicId), "topicId")]
public partial class TopicChatViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly ITopicCommentsService _svc;

    [ObservableProperty] private string topicId = "";
    [ObservableProperty] private string error = "";
    [ObservableProperty] private string newMessage = "";
    [ObservableProperty] private bool isBusy;

    public ObservableCollection<TopicComment> Items { get; } = new();

    public TopicChatViewModel(IAuthService auth, ITopicCommentsService svc)
    {
        _auth = auth;
        _svc = svc;
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
            if (string.IsNullOrWhiteSpace(TopicId))
            {
                Error = "TopicId missing.";
                return;
            }

            var list = await _svc.GetCommentsAsync(TopicId);
            foreach (var c in list)
                Items.Add(c);
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

        if (string.IsNullOrWhiteSpace(TopicId))
        {
            Error = "TopicId missing.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewMessage))
            return;

        await _svc.AddCommentAsync(TopicId, NewMessage, user);
        NewMessage = "";
        await LoadAsync();
    }
}
