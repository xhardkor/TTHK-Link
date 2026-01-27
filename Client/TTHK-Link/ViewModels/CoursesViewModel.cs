using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

public partial class CoursesViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly ICourseService _courses;

    public ObservableCollection<Course> Items { get; } = new();

    [ObservableProperty] private string error = "";
    [ObservableProperty] private bool isBusy;

    public CoursesViewModel(IAuthService auth, ICourseService courses)
    {
        _auth = auth;
        _courses = courses;
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
            var user = _auth.CurrentUser;
            if (user == null)
            {
                Error = "Not authenticated.";
                await Shell.Current.GoToAsync("//login");
                return;
            }

            var list = await _courses.GetCoursesForUserAsync(user);

            foreach (var c in list)
                Items.Add(c);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            Error = "Failed to load courses.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task OpenCourseAsync(Course course)
    {
        if (course == null) return;

        if (course == null) return;

        try
        {
            var id = Uri.EscapeDataString(course.Id);
            var name = Uri.EscapeDataString(course.CourseName);

            await Shell.Current.GoToAsync($"courseTopics?courseId={id}&courseName={name}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Navigation error", ex.Message, "OK");
        }
    }
}
