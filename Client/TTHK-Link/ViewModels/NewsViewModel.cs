using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TTHK_Link.ViewModels;

public partial class NewsViewModel : ObservableObject
{
    private readonly INewsService _news;

    public ObservableCollection<News> Items { get; } = new();

    [ObservableProperty] private string error = "";

    public NewsViewModel(INewsService news)
    {
        _news = news;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            Error = "";
            Items.Clear();
            var list = await _news.GetLatestNewsAsync(20);
            foreach (var n in list) Items.Add(n);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    public async Task OpenAsync(News item)
    {
        if (item == null) return;

        // откроет ссылку как в телеге (браузер)
        await Launcher.Default.OpenAsync(item.Link);
    }
}
