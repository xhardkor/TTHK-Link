using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

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
        await Vm.LoadAsync();
    }
}
