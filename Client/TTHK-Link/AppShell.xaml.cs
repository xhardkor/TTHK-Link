using TTHK_Link.Pages;

namespace TTHK_Link;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("chat", typeof(ChatPage));    
    }
}