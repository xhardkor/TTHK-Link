using TTHK_Link.Pages;
using TTHK_Link.ViewModels;

namespace TTHK_Link;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        //Routing.RegisterRoute("chat", typeof(ChatPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("profile", typeof(ProfilePage));
        Routing.RegisterRoute("courseTopics", typeof(CourseTopicsPage));
        Routing.RegisterRoute("topicChat", typeof(TopicChatPage));
        Routing.RegisterRoute("news", typeof(NewsPage));
    }
}
