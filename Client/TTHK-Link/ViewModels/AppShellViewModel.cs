using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private bool isAuthenticated;

    [ObservableProperty]
    private FlyoutBehavior flyoutBehavior;

    [ObservableProperty]
    private string login = "";

    [ObservableProperty]
    private string avatarLetter = "?";

    [ObservableProperty]
    private string statusText = "";


    public AppShellViewModel(IAuthService authService)
    {
        _authService = authService;
        RefreshAuthState();
    }

    public void RefreshAuthState()
    {
        var user = _authService.CurrentUser;

        IsAuthenticated = user != null;
        FlyoutBehavior = IsAuthenticated
            ? FlyoutBehavior.Flyout
            : FlyoutBehavior.Disabled;

        Login = user?.Login ?? "";
        StatusText = user?.Status ?? "";
        AvatarLetter = string.IsNullOrWhiteSpace(Login)
            ? "?"
            : Login[..1].ToUpperInvariant();
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.LogoutAsync();
        RefreshAuthState();
        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync("//login");
    }
}
