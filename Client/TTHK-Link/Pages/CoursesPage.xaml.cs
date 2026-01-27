using TTHK_Link.Models;
using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class CoursesPage : ContentPage
{
    private CoursesViewModel Vm => (CoursesViewModel)BindingContext;

    private bool _opening;

    public CoursesPage(CoursesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Vm.LoadAsync();
    }

    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (_opening) return;

        var course = e.CurrentSelection?.FirstOrDefault() as Course;
        if (course == null) return;

        if (sender is CollectionView cv)
            cv.SelectedItem = null;

        try
        {
            _opening = true;
            await Vm.OpenCourseAsync(course);
        }
        finally
        {
            _opening = false;
        }
    }
}
