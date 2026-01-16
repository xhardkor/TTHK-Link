using Microsoft.Extensions.DependencyInjection;
using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class TopicChatPage : ContentPage, IQueryAttributable
{
    private TopicChatViewModel Vm => (TopicChatViewModel)BindingContext;

    public TopicChatPage() : this(App.Current!.Handler!.MauiContext!.Services.GetRequiredService<TopicChatViewModel>())
    {
    }

    public TopicChatPage(TopicChatViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        foreach (var kv in query)
            System.Diagnostics.Debug.WriteLine($"TOPIC QUERY: {kv.Key} = {kv.Value}");

        Vm.TopicId = GetQueryValue(query, "topicId");

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Vm.LoadAsync();
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrWhiteSpace(Vm.TopicId))
            await Vm.LoadAsync();
    }

    private static string GetQueryValue(IDictionary<string, object> query, string key)
    {
        foreach (var kv in query)
        {
            if (string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase))
                return kv.Value?.ToString() ?? "";
        }

        return "";
    }
}
