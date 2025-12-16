namespace TTHK_Link.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string CourseId { get; set; } // course id 
        public string UserId { get; set; } // - 

        public string Msg { get; set; } //+
        public string? ImageUrl { get; set; }   // сделай nullable
        public DateTime CreatedAt { get; set; } // + 

        //ui helper
        public bool IsMine { get; set; }
    }   
}
