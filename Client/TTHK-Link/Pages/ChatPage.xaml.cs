using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class ChatPage : ContentPage
{
    public ChatPage(ChatViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm; // Seome ViewModeli lehega
    }
}
