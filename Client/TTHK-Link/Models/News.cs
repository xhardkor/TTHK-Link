namespace TTHK_Link.Models
{
    public class News
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Link { get; set; } = "";
        public DateTime PublishDate { get; set; }
        public string Summary { get; set; } = "";

        public string ImageUrl { get; set; } = ""; 
    }
}
