using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTHK_Link.Pages;
using TTHK_Link.Services;
using TTHK_Link.Services.Fake;
using TTHK_Link.Services.Http;
using TTHK_Link.Services.Interfaces;
using TTHK_Link.ViewModels;


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

        // FAKE AUTH SERVICE
        //builder.Services.AddSingleton<IAuthService, FakeAuthService>();


        builder.Services.AddTransient<HttpLoggingHandler>();

        //// REAL AUTOH SERVICE
        builder.Services.AddHttpClient<ApiAuthService>(c =>
        {
            // Serveri aadress (sama mis Postmanis)
            c.BaseAddress = new Uri("http://172.20.10.2:8080");
        }).AddHttpMessageHandler<HttpLoggingHandler>();

        builder.Services.AddSingleton<IAuthService>(sp =>
            sp.GetRequiredService<ApiAuthService>());

        //menu shell
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<AppShellViewModel>();
        



        builder.Services.AddSingleton<ICourseService, FakeCourseService>();
        builder.Services.AddSingleton<IChatService, FakeChatService>();
        builder.Services.AddSingleton<IUserService, FakeUserService>();


        // view models
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<CourseViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddSingleton<ChatListViewModel>();
        builder.Services.AddTransient<ChatListPage>();
        builder.Services.AddTransient<RegisterViewModel>();

        // pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CoursePage>();
        builder.Services.AddTransient<ChatPage>();
        builder.Services.AddSingleton<FlyoutViewModel>();
        builder.Services.AddTransient<NewsPage>();
        builder.Services.AddSingleton<TTHK_Link.Services.IThemeService, TTHK_Link.Services.ThemeService>();
        builder.Services.AddSingleton<TTHK_Link.ViewModels.ThemeViewModel>();
        builder.Services.AddTransient<TTHK_Link.Pages.ThemePage>();
        builder.Services.AddTransient<TTHK_Link.Pages.ProfilePage>();
        //shell
        builder.Services.AddSingleton<AppShell>();
        
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ProfilePage>();

        //session cache
        builder.Services.AddSingleton<ISessionCache, SessionCache>();






        return builder.Build();
    }
}