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




        // =======================
        // AUTH MODE: FAKE / REAL
        // =======================

        //// FAKE AUTH SERVICE
        //builder.Services.AddSingleton<IAuthService, FakeAuthService>();


        builder.Services.AddTransient<HttpLoggingHandler>();


        // =======================

        //// REAL AUTOH SERVICE

        builder.Services.AddHttpClient<ApiAuthService>(c =>
        {
            c.BaseAddress = new Uri("http://192.168.93.141:8080");
        }).AddHttpMessageHandler<HttpLoggingHandler>();

        builder.Services.AddHttpClient<ApiChatService>(c =>
        {
            c.BaseAddress = new Uri("http://192.168.93.141:8080");
        }).AddHttpMessageHandler<HttpLoggingHandler>();

        builder.Services.AddHttpClient<ApiCourseService>(c =>
        {
            c.BaseAddress = new Uri("http://192.168.93.141:8080"); // 
        }).AddHttpMessageHandler<HttpLoggingHandler>();

        builder.Services.AddSingleton<ICourseService>(sp =>
            sp.GetRequiredService<ApiCourseService>());




        builder.Services.AddSingleton<IAuthService>(sp =>
            sp.GetRequiredService<ApiAuthService>());

        // =======================




        //menu shell
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<AppShellViewModel>();
        



        //builder.Services.AddSingleton<ICourseService, FakeCourseService>();
        //builder.Services.AddSingleton<IChatService, InMemoryChatService>();
        //builder.Services.AddSingleton<IUserService, FakeUserService>();
        //builder.Services.AddSingleton<ICourseTopicsService, FakeCourseTopicsService>();
        //builder.Services.AddSingleton<ITopicCommentsService, FakeTopicCommentsService>();
        builder.Services.AddSingleton<INewsService, FakeNewsService>();

        builder.Services.AddSingleton<IChatService>(sp =>
            sp.GetRequiredService<ApiChatService>());

        
        



        // view models
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<CoursesViewModel>();
        builder.Services.AddTransient<TTHK_Link.ViewModels.SettingsViewModel>();
        builder.Services.AddTransient<TTHK_Link.Pages.SettingsPage>();
        //builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<CourseTopicsViewModel>();
        builder.Services.AddTransient<TopicChatViewModel>();
        builder.Services.AddTransient<NewsViewModel>();








        // pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CoursesPage>();
        //builder.Services.AddTransient<ChatPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<CoursesPage>();
        builder.Services.AddTransient<CourseTopicsPage>();
        builder.Services.AddTransient<TopicChatPage>();

        builder.Services.AddTransient<GroupChatViewModel>();
        builder.Services.AddTransient<GroupChatPage>();
        builder.Services.AddTransient<NewsPage>();



        //session cache
        builder.Services.AddSingleton<ISessionCache, SessionCache>();






        return builder.Build();
    }
}