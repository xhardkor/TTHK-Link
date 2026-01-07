using TTHK_Link.Services;

namespace TTHK_Link;

public partial class App : Application
{
    public App(AppShell shell, IThemeService theme)
    {
        InitializeComponent();

        theme.Apply(theme.CurrentMode);

        MainPage = shell;
    }
}