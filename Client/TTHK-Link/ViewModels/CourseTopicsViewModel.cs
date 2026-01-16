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
    private readonly ICourseTopicsService _topics;

    [ObservableProperty] private string courseId = "";
    [ObservableProperty] private string courseName = "";
    [ObservableProperty] private string error = "";
    [ObservableProperty] private bool isBusy;

    public ObservableCollection<CourseTopic> Items { get; } = new();

    public CourseTopicsViewModel(IAuthService auth, ICourseTopicsService topics)
    {
        _auth = auth;
        _topics = topics;
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

            var list = await _topics.GetTopicsAsync(CourseId);
            foreach (var t in list)
                Items.Add(t);
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

        await _topics.CreateTopicAsync(CourseId, title, body, user);
        await LoadAsync();
    }

    [RelayCommand]
    public async Task OpenTopicAsync(CourseTopic topic)
    {
        if (topic == null) return;

        var id = Uri.EscapeDataString(topic.Id);
        await Shell.Current.GoToAsync($"topicChat?topicId={id}");
    }
}
