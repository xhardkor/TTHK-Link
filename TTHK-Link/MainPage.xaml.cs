namespace TTHK_Link;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} xd dddddddd";
        else
            CounterBtn.Text = $"Clicked {count} xdddd heheheehehhe";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}