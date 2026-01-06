using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TTHK_Link.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    [ObservableProperty] private string fullName = "User";
    [ObservableProperty] private string groupName = "Group";
    [ObservableProperty] private string profileInitials = "U";

    public ShellViewModel(IAuthService auth)
    {
        _auth = auth;
        Refresh();
    }

    public void Refresh()
    {
        var u = _auth.CurrentUser;
        if (u == null)
        {
            FullName = "Guest";
            GroupName = "";
            ProfileInitials = "G";
            return;
        }

        // Подстрой под свои поля модели User:
        // например u.FirstName, u.LastName, u.GroupName
        FullName = $"{u.FirstName} {u.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(FullName))
            FullName = u.Name ?? u.Email ?? "User";

        GroupName = u.GroupName ?? u.Group ?? ""; // подстрой под твой User.cs
        ProfileInitials = GetInitials(FullName);
    }

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "U";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0][0].ToString().ToUpperInvariant();
        return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}";
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _auth.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}