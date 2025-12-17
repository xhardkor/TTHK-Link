namespace TTHK_Link.Pages;
using TTHK_Link.ViewModels;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}