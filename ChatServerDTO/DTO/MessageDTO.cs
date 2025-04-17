using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerDTO.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }

        public int IdChat { get; set; }

        public string Text { get; set; } = null!;

        public int IdSender { get; set; }

        public bool? IsReadIt { get; set; }

        public byte[]? Document { get; set; }

        public string? DocumentTitle { get; set; }

        public ChatDTO Chat { get; set; } = null!;

        public UserDTO Sender { get; set; } = null!;

        public static explicit operator MessageDTO(Message message)
        {
            var result = new MessageDTO
            {
                Id = message.Id,
                IdChat = message.IdChat,
                Text = message.Text,
                Document = message.Document,
                DocumentTitle = message.DocumentTitle,
                IdSender = message.IdSender,
                IsReadIt = message.IsReadIt
            };

            if (message.IdChatNavigation != null)
                result.Chat = (ChatDTO)message.IdChatNavigation;

            if (message.IdSenderNavigation != null)
                result.Sender = (UserDTO)message.IdSenderNavigation;

            return result;
        }

        public static explicit operator Message(MessageDTO from)
        {
            var result = new Message
            {
                Id = from.Id,
                IdChat = from.IdChat,
                Text = from.Text,
                Document = from.Document,
                DocumentTitle = from.DocumentTitle,
                IdSender = from.IdSender,
                IsReadIt = from.IsReadIt

            };
            return result;
        }
    }
}
