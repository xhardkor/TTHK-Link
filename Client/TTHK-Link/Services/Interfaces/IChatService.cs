using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces
{
    internal interface IChatService
    {
        Task<IReadOnlyList<Message>> GetMessagesAsync(string groupId);
        Task<Message> SendMessageAsync(string groupId, string text, string imagePath = null);
    }
}
