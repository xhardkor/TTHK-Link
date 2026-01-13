using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(FlyoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        vm.RefreshProfile(); // на всякий случай обновим данные
    }
}