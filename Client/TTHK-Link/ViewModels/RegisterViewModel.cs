using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;

namespace TTHK_Link.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string groupId = "";
    [ObservableProperty] private string error = "";

    public RegisterViewModel(IAuthService auth)
    {
        _auth = auth;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        Error = "";

        var ok = await _auth.RegisterAsync(new RegisterRequest
        {
            Username = Username,
            Password = Password,
            GroupId = GroupId
        });

        if (!ok)
        {
            Error = "Registreerimine ebaõnnestus (kasutaja olemas või andmed puudu).";
            return;
        }

        // Pärast registreerimist läheme kursustele
        await Shell.Current.GoToAsync("//groups");
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        // Tagasi login vaatesse
        await Shell.Current.GoToAsync("//login");
    }
}
