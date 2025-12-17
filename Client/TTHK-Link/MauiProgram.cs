using Java.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTHK_Link.Pages;
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

        // services
        //builder.Services.AddSingleton<IAuthService, FakeAuthService>();

        builder.Services.AddHttpClient<ApiAuthService>(c =>
        {
            // Serveri aadress (sama mis Postmanis)
            c.BaseAddress = new Uri("http://172.20.10.2:8080");
        });

        builder.Services.AddSingleton<IAuthService>(sp =>
            sp.GetRequiredService<ApiAuthService>());

        builder.Services.AddSingleton<ICourseService, FakeCourseService>();
        builder.Services.AddSingleton<IChatService, FakeChatService>();
        builder.Services.AddSingleton<IUserService, FakeUserService>();


        // view models
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<CourseViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        // pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CoursePage>();
        builder.Services.AddTransient<ChatPage>();
        builder.Services.AddTransient<RegisterPage>();


        System.Diagnostics.Debug.WriteLine($"REGISTER URL: {_http.BaseAddress}{url}");



        return builder.Build();
    }
}