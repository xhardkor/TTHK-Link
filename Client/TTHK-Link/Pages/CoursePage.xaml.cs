using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class CoursePage : ContentPage
{
    private readonly CourseViewModel _vm;

    public CoursePage(CourseViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
