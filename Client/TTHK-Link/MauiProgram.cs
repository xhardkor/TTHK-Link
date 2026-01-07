using Microsoft.Extensions.Logging;
using TTHK_Link.Services.Interfaces;
using TTHK_Link.Services.Fake;
using TTHK_Link.ViewModels;
using TTHK_Link.Pages;

namespace TTHK_Link;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();


        builder
            .UseMauiApp<App>()              // 
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        // …

        // services
        builder.Services.AddSingleton<IAuthService, FakeAuthService>();
        builder.Services.AddSingleton<ICourseService, FakeCourseService>();
        builder.Services.AddSingleton<IChatService, FakeChatService>();
        builder.Services.AddSingleton<IUserService, FakeUserService>();


        // view models
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<CourseViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddSingleton<ChatListViewModel>();
        builder.Services.AddTransient<ChatListPage>();
        // pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CoursePage>();
        builder.Services.AddTransient<ChatPage>();
        builder.Services.AddSingleton<FlyoutViewModel>();
        builder.Services.AddTransient<NewsPage>();
        builder.Services.AddSingleton<TTHK_Link.Services.IThemeService, TTHK_Link.Services.ThemeService>();
        builder.Services.AddSingleton<TTHK_Link.ViewModels.ThemeViewModel>();
        builder.Services.AddTransient<TTHK_Link.Pages.ThemePage>();
        
        //shell
        builder.Services.AddSingleton<AppShell>();
        
        return builder.Build();
    }
}