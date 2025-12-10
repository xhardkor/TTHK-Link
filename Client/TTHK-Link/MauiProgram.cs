using Microsoft.Extensions.Logging;
using TTHK_Link.Services.Interfaces;
using TTHK_Link.Services.Fake;

namespace TTHK_Link;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // …

        builder.Services.AddSingleton<IAuthService, FakeAuthService>();
        builder.Services.AddSingleton<IGroupService, FakeGroupService>();
        builder.Services.AddSingleton<IChatService, FakeChatService>();

        builder.Services.AddTransient<LoginViewModel>();

        return builder.Build();
    }
}