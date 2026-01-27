using TTHK_Link.Models;
using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class NewsPage : ContentPage
{
    private NewsViewModel Vm => (NewsViewModel)BindingContext;

    public NewsPage(NewsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Vm.LoadAsync();
    }

    private async void OnSelected(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection?.FirstOrDefault() as News;
        if (item == null) return;

        if (sender is CollectionView cv)
            cv.SelectedItem = null; // Android

        await Vm.OpenAsync(item);
    }
}
