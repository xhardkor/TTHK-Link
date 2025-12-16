using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

public partial class CourseViewModel : ObservableObject
{
    private readonly ICourseService _courses;
    private readonly IAuthService _auth;

    public ObservableCollection<Course> Items { get; } = new();

    public CourseViewModel(ICourseService courses, IAuthService auth)
    {
        _courses = courses;
        _auth = auth;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        Items.Clear();

        var user = _auth.CurrentUser;
        if (user == null)
            return;

        var list = await _courses.GetCoursesForUserAsync(user);

        foreach (var g in list)
            Items.Add(g);
    }
}
