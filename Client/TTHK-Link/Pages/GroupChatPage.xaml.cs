namespace TTHK_Link.Pages;
using TTHK_Link.ViewModels;

public partial class GroupChatPage : ContentPage
{
    private GroupChatViewModel Vm => (GroupChatViewModel)BindingContext;

    public GroupChatPage(GroupChatViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("=== GroupChatPage OnAppearing ===");

        if (Vm.Items.Count == 0 && !Vm.IsBusy)
            await Vm.LoadAsync();

        Vm.StartPolling(); 
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Vm.StopPolling(); // 
    }

    private async void OnEntryCompleted(object sender, EventArgs e)
    {
        if (Vm.CanSendMessage)
            await Vm.SendMessageAsync();
    }
}

