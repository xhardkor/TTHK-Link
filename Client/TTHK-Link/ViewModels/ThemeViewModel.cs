using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Services;

namespace TTHK_Link.ViewModels;

public partial class ThemeViewModel : ObservableObject
{
    private readonly IThemeService _theme;

    [ObservableProperty] private AppThemeMode current;

    public ThemeViewModel(IThemeService theme)
    {
        _theme = theme;
        Current = _theme.CurrentMode;
    }

    [RelayCommand]
    private void SetTheme(string mode)
    {
        var m = mode switch
        {
            "System" => AppThemeMode.System,
            "Light"  => AppThemeMode.Light,
            "Dark"   => AppThemeMode.Dark,
            _        => AppThemeMode.System
        };

        _theme.Apply(m);
        Current = m;
        System.Diagnostics.Debug.WriteLine($"THEME SET: {m}");
    }
}