using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private bool isBusy;

    // Veateade kasutajale (näitame lehel11111)
    [ObservableProperty] private string error = "";

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        // Kui juba töötab, ei käivita uuesti
        if (IsBusy) return;

        IsBusy = true;
        Error = "";

        try
        {
            // Proovime sisse logida (fake auth)
            var ok = await _authService.LoginAsync(new LoginRequest
            {
                Username = Username,
                Password = Password
            });

            // Kui login ebaõnnestus, näitame veateadet
            if (!ok)
            {
                Error = "Vale kasutajanimi või parool (proovi admin/admin)";
                return;
            }

            // Edukas login → absoluutne navigeerimine gruppide lehele
            await Shell.Current.GoToAsync("//groups");
        }
        catch (Exception ex)
        {
            // Kui midagi läheb valesti (nt route puudub), näitame vea teksti
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
