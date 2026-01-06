namespace TTHK_Link.Models;

public class FlyoutMenuItem
{
    public string Section { get; set; } = "";
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "";   // png в Resources/Images
    public string Route { get; set; } = "";  // "groups", "chat", ...
    public string? Badge { get; set; }       // "2", "!" и т.п.
}