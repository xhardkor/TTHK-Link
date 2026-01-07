using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;

namespace TTHK_Link.ViewModels;

public partial class FlyoutViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    public ObservableCollection<FlyoutMenuItem> Items { get; } = new();

    [ObservableProperty] private string fullName = "User";
    [ObservableProperty] private string groupName = "";
    [ObservableProperty] private string profileInitials = "U";
    [ObservableProperty] private string? avatarUrl;

    public FlyoutViewModel(IAuthService auth)
    {
        _auth = auth;

        BuildMenu();
        RefreshProfile();
    }

    public void RefreshProfile()
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

        FullName = string.IsNullOrWhiteSpace(u.SenderName) ? "User" : u.SenderName!;
        GroupName = u.GroupId;
        AvatarUrl = u.ImageUrl;

        ProfileInitials = GetInitials(FullName);
    }

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "U";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();
        return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}";
    }

    private void BuildMenu()
    {
        Items.Clear();

        Items.Add(new FlyoutMenuItem
        {
            Section = "Основное",
            Title = "Новости",
            Icon = "news.png",
            Route = "news"
        });

        Items.Add(new FlyoutMenuItem
        {
            Section = "Основное",
            Title = "Чат",
            Icon = "chat.png",
            Route = "chatlist"
        });

        Items.Add(new FlyoutMenuItem
        {
            Section = "Приложение",
            Title = "Оформление",
            Icon = "design.png",
            Route = "theme"
        });

        Items.Add(new FlyoutMenuItem
        {
            Section = "Приложение",
            Title = "О приложении",
            Icon = "info.png",
            Route = "about"
        });
    }

    [RelayCommand]
    private async Task OpenAsync(FlyoutMenuItem item)
    {
        if (item is null) return;

        // закрыть меню
        Shell.Current.FlyoutIsPresented = false;

        // если маршрута нет — просто игнор (например, пока не сделано)
        if (string.IsNullOrWhiteSpace(item.Route)) return;

        await Shell.Current.GoToAsync($"//{item.Route}");
    }

    [RelayCommand]
    private async Task OpenProfileAsync()
    {
        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync("//profile");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _auth.LogoutAsync();
        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync("//login");
    }
}