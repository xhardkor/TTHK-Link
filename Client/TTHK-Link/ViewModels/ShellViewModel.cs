using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;

namespace TTHK_Link.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string groupName = "";
    [ObservableProperty] private string profileInitials = "U";
    [ObservableProperty] private string? avatarUrl;

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
            AvatarUrl = null;
            return;
        }

        // 1 Имя
        FullName = string.IsNullOrWhiteSpace(u.SenderName)
            ? "User"
            : u.SenderName;

        // 2 Группа (у тебя это GroupId)
        GroupName = u.GroupId;

        // 3 Аватар
        AvatarUrl = u.ImageUrl;

        // 4 Инициалы
        ProfileInitials = GetInitials(FullName);
    }

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "U";

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
            return parts[0].Substring(0, 1).ToUpperInvariant();

        return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}";
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _auth.LogoutAsync();
        await Shell.Current.GoToAsync("//login");
    }
}