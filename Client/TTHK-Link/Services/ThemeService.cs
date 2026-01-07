using Microsoft.Maui.Storage;

namespace TTHK_Link.Services;

public enum AppThemeMode
{
    System = 0,
    Light = 1,
    Dark = 2
}

public interface IThemeService
{
    AppThemeMode CurrentMode { get; }
    void Apply(AppThemeMode mode);
}

public class ThemeService : IThemeService
{
    private const string Key = "app_theme_mode";

    public AppThemeMode CurrentMode =>
        (AppThemeMode)Preferences.Get(Key, (int)AppThemeMode.System);

    public void Apply(AppThemeMode mode)
    {
        Preferences.Set(Key, (int)mode);

        Application.Current!.UserAppTheme = mode switch
        {
            AppThemeMode.Light => AppTheme.Light,
            AppThemeMode.Dark  => AppTheme.Dark,
            _                  => AppTheme.Unspecified
        };
    }
}