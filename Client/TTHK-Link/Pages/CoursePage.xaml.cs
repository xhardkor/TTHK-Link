using TTHK_Link.ViewModels;
using TTHK_Link.Models;

namespace TTHK_Link.Pages;


public partial class CoursePage : ContentPage
{
    private CourseViewModel Vm => (CourseViewModel)BindingContext;

    public CoursePage(CourseViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Vm.LoadAsync(); // Laeme kursused
    }

    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        var course = (Course)e.CurrentSelection[0];
        ((CollectionView)sender).SelectedItem = null;

        try
        {
            // Teeme ID URL-safe kujule
            var id = Uri.EscapeDataString(course.Id);

            // NB! Siin EI kasuta // ega ///, sest 'chat' on registreeritud route
            await Shell.Current.GoToAsync($"Chat?courseId={id}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Viga", ex.Message, "OK");
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }


}
