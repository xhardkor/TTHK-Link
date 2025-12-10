using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

   
    [ObservableProperty]
    private string username = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private bool isBusy;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var result = await _authService.LoginAsync(new LoginRequest
            {
                Username = Username,  
                Password = Password
            });

            // TODO: if ok go to main page
            //  await Shell.Current.GoToAsync("//MainPage");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
