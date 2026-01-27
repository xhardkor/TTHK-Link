using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTHK_Link.Models
{
    public class TopicComment
    {
        public string Id { get; set; } = "";
        public string TopicId { get; set; } = "";
        public string UserId { get; set; } = "";
        public string AuthorLogin {  get; set; }
        public string Text { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
