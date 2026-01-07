using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class ThemePage : ContentPage
{
    public ThemePage(ThemeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnTestLight(object sender, EventArgs e)
    {
        Application.Current!.UserAppTheme = AppTheme.Light;
    }

    private void OnTestDark(object sender, EventArgs e)
    {
        Application.Current!.UserAppTheme = AppTheme.Dark;
    }
}