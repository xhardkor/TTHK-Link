using TTHK_Link.Pages;

namespace TTHK_Link;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("Login", typeof(LoginPage));
        Routing.RegisterRoute("Course", typeof(CoursePage));
        Routing.RegisterRoute("Chat", typeof(ChatPage));
    }
}