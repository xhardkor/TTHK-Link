using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TTHK_Link.Models;

namespace TTHK_Link.ViewModels;

public partial class ChatListViewModel : ObservableObject
{
    public ObservableCollection<ChatThread> Items { get; } = new();

    public ChatListViewModel()
    {
        LoadDemo();
    }

    private void LoadDemo()
    {
        Items.Clear();

        Items.Add(new ChatThread { Id="general", Title="Общий чат", LastMessage="Последнее сообщение…", LastAt=DateTime.Now.AddMinutes(-3), UnreadCount=2 });
        Items.Add(new ChatThread { Id="tthk24", Title="TTHK-24", LastMessage="Завтра пара в 9:00", LastAt=DateTime.Now.AddHours(-1), UnreadCount=0 });
        Items.Add(new ChatThread { Id="admins", Title="Админы", LastMessage="Проверили доступы", LastAt=DateTime.Now.AddDays(-1), UnreadCount=5 });
    }

    [RelayCommand]
    private async Task OpenChatAsync(ChatThread chat)
    {
        if (chat is null) return;

        // открываем конкретный чат
        await Shell.Current.GoToAsync($"//chat?chatId={Uri.EscapeDataString(chat.Id)}&title={Uri.EscapeDataString(chat.Title)}");
    }
}