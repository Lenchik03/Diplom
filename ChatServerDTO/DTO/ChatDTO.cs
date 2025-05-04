using ChatServerDTO.DB;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerDTO.DTO
{
    public class ChatDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public byte[]? ImagePath { get; set; }

        public string? ImageSourse { get; set; }
        public int? IdCreator { get; set; }

        public bool? IsDeleted { get; set; }

        public UserDTO? Creator { get; set; }

        public List<ChatUserDTO> ChatUsers { get; set; } = new List<ChatUserDTO>();

        public List<MessageDTO> Messages { get; set; } = new List<MessageDTO>();

        public static explicit operator ChatDTO(Chat from)
        {
            var result = new ChatDTO { 
                Id = from.Id,
                Title = from.Title,
                ImagePath = from.ImagePath,
                IsDeleted = from.IsDeleted,
            };

            if (from.IdCreatorNavigation != null)
                result.Creator = (UserDTO)from.IdCreatorNavigation;

            if (from.ChatUsers != null)
                result.ChatUsers = from.ChatUsers.Select(s => (ChatUserDTO)s).ToList();

            if (from.Messages != null)
                result.Messages = from.Messages.Select(s => (MessageDTO)s).ToList();

            return result;
        }

        public static explicit operator Chat(ChatDTO chat)
        {
            var result = new Chat
            {
                Id = chat.Id,
                Title = chat.Title,
                ImagePath = chat.ImagePath

            };
            if (chat.ChatUsers != null)
                result.ChatUsers = chat.ChatUsers.Select(s => (ChatUser)s).ToList();
            return result;
        }
    }
}
