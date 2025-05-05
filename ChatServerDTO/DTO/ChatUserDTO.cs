using ChatServerDTO.DB;
using ChatServerDTO.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerDTO.DTO
{
    public class ChatUserDTO
    {
        public int Id { get; set; }

        public int IdChat { get; set; }

        public int IdUser { get; set; }

        //public virtual ChatDTO IdChatNavigation { get; set; }

        public virtual UserDTO User { get; set; }

        public static explicit operator ChatUserDTO(ChatUser chatUser)
        {
            var result = new ChatUserDTO
            {
                Id = chatUser.Id,
                IdChat = chatUser.IdChat,
                IdUser = chatUser.IdUser,

            };

            if (chatUser.IdUserNavigation != null)
                result.User = (UserDTO)chatUser.IdUserNavigation;
            
            return result;
        }

        public static explicit operator ChatUser(ChatUserDTO chatUserDTO)
        {
            var result = new ChatUser
            {
                Id = chatUserDTO.Id,
                IdChat = chatUserDTO.IdChat,
                IdUser = chatUserDTO.IdUser
            }; 
            
            if (chatUserDTO.User != null)
                result.IdUserNavigation = (User)chatUserDTO.User;

            return result;
        }
    }
}
