using CommunityToolkit.Mvvm.ComponentModel;

namespace TTHK_Link.ViewModels;

// Simple settings VM: only state, no business logic yet
public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isDarkMode;

    [ObservableProperty]
    private bool pushNotifications;

    [ObservableProperty]
    private bool emailNotifications;

    [ObservableProperty]
    private string language = "English";
}