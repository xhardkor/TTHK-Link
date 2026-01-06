using TTHK_Link.ViewModels;

namespace TTHK_Link;

public partial class AppShell : Shell
{
    private readonly FlyoutViewModel _vm;
    private readonly IAuthService _auth;

    public AppShell(FlyoutViewModel vm, IAuthService auth)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _vm.RefreshProfile();

        if (_auth.CurrentUser == null)
            await GoToAsync("//login");
    }
}