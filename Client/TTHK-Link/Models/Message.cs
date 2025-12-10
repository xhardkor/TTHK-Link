namespace TTHK_Link.Models
{
    internal class Message
    {
        public string Id { get; set; }
        public string GroupId { get; set; }     
        public string UserId { get; set; }

        public string Text { get; set; }
        public string ImageUrl { get; set; }     // null, if no image
        public DateTime CreatedAt { get; set; }

        //????
        public bool IsMine { get; set; }
    }
}
