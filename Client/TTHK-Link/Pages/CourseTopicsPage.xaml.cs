using Microsoft.Extensions.DependencyInjection;
using TTHK_Link.Models;
using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class CourseTopicsPage : ContentPage, IQueryAttributable
{
    private CourseTopicsViewModel Vm => (CourseTopicsViewModel)BindingContext;

    public CourseTopicsPage() : this(App.Current!.Handler!.MauiContext!.Services.GetRequiredService<CourseTopicsViewModel>())
    {
    }

    public CourseTopicsPage(CourseTopicsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Debug what actually arrived
        foreach (var kv in query)
            System.Diagnostics.Debug.WriteLine($"QUERY: {kv.Key} = {kv.Value}");

        Vm.CourseId = GetQueryValue(query, "courseId");
        Vm.CourseName = Uri.UnescapeDataString(GetQueryValue(query, "courseName"));

        // Load AFTER query applied (important)
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Vm.LoadAsync();
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Avoid loading too early (when CourseId not applied yet)
        if (!string.IsNullOrWhiteSpace(Vm.CourseId))
            await Vm.LoadAsync();
    }

    private static string GetQueryValue(IDictionary<string, object> query, string key)
    {
        // Case-insensitive lookup
        foreach (var kv in query)
        {
            if (string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase))
                return kv.Value?.ToString() ?? "";
        }

        return "";
    }

    private async void OnSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        var topic = (CourseTopic)e.CurrentSelection[0];
        ((CollectionView)sender).SelectedItem = null;

        await Vm.OpenTopicAsync(topic);
    }

    private async void OnTopicTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not CourseTopic topic)
            return;

        await Vm.OpenTopicAsync(topic);
    }



}
