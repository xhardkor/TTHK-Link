namespace TTHK_Link.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }

        public string Text { get; set; }
        public string? ImageUrl { get; set; }   // сделай nullable
        public DateTime CreatedAt { get; set; }
        
        //ui
        public bool IsMine { get; set; }
    }
}
