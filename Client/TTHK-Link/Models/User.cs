namespace TTHK_Link.Models
{
    public class User
    {
        public string Id { get; set; } = default!;
        public string Login { get; set; } = default!;
        public bool IsAdmin { get; set; }
        public string GroupId { get; set; } //TiTge24 - rühma ID

        public string? ImageUrl { get; set; }

        // UI helper
        public bool IsMine { get; set; }

        // UI jaoks: saatja nimi
        public string? SenderName { get; set; }
    }

}
