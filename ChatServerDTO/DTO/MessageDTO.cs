using ChatServerDTO.DB;
using ChatServerDTO.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace ChatServerDTO.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }

        public int IdChat { get; set; }

        public string? Text { get; set; } = string.Empty;

        public int IdSender { get; set; }

        public byte[]? Document { get; set; }

        public string? DocumentTitle { get; set; }
        public DateTime? DateOfSending { get; set; }
        public bool? IsChanged { get; set; } = false;

        public bool? IsDeleted { get; set; } = false;
        public string? ChangedTitle { get; set; } = "";
        public string? DeletedTitle { get; set; } = "";
        //public ChatDTO Chat { get; set; } = null!;

        public UserDTO? Sender { get; set; }

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
                DateOfSending = message.DateOfSending,
                IsChanged = message.IsChanged,
                IsDeleted = message.IsDeleted
            };
            if (message.IsChanged != null)
                result.ChangedTitle = "(ред).";
            if (message.IsDeleted != null)
                result.DeletedTitle = "Сообщение удалено!";
            /*
            if (message.IdChatNavigation != null)
                result.Chat = (ChatDTO)message.IdChatNavigation;

            if (message.IdSenderNavigation != null)
                result.Sender = (UserDTO)message.IdSenderNavigation;
            */
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
                DateOfSending = from.DateOfSending,
                IsChanged= from.IsChanged,
                IsDeleted = from.IsDeleted
            };
            return result;
        }
    }
}
