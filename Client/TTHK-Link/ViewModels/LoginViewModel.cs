using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string error = "";

    public LoginViewModel(IAuthService auth)
    {
        _auth = auth;
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        Error = "";

        var ok = await _auth.LoginAsync(new LoginRequest
        {
            Username = Username,
            Password = Password
        });

        if (!ok)
        {
            Error = "Kasutajat ei leitud või parool on vale.";
            return;
        }

        var shellVm = App.Current?.Handler?.MauiContext?.Services.GetService<AppShellViewModel>();
        shellVm?.RefreshAuthState();

        // Pärast edukat login'it suuname kursustele
        await Shell.Current.GoToAsync("//courses");
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        Error = "";
        await Shell.Current.GoToAsync("register");
    }

}
