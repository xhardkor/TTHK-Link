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

    public AppShellViewModel(IAuthService authService)
    {
        _authService = authService;
        RefreshAuthState();
    }

    public void RefreshAuthState()
    {
        IsAuthenticated = _authService.CurrentUser != null;
        FlyoutBehavior = IsAuthenticated ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
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
