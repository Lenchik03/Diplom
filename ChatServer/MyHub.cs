using ChatServerDTO.DTO;
using Microsoft.AspNetCore.SignalR;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ChatServer
{
    public class MyHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnectedAsync()
        {
            
            return base.OnConnectedAsync();
        }
        private MyHub myHub;
        public async Task<ObservableCollection<UserDTO>> GetChatUsers(int chatId)
        {
            var result = await REST.Instance.client.GetAsync($"ChatMembers/{chatId}");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                return await result.Content.ReadFromJsonAsync<ObservableCollection<UserDTO>>(REST.Instance.options);
            }
        }
        public async System.Threading.Tasks.Task NewMessage(UserDTO sender, MessageDTO message, ChatDTO chat)
        {
            var chatUsers = await GetChatUsers(chat.Id);
            message.IdChat = chat.Id;
            message.Chat = chat;
            message.IdSender = sender.Id;
            message.Sender = sender;

            string arg = JsonSerializer.Serialize(message, REST.Instance.options);
            var responce = await REST.Instance.client.PostAsync($"Messages",
                new StringContent(arg, Encoding.UTF8, "application/json"));
            try
            {
                responce.EnsureSuccessStatusCode();
                //MessageBox.Show("Задача успешно обновлена!");

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Ошибка! Обновление задачи приостановлено!");
                return;
            }

            chatUsers.Select(s => s.Id).ToList().ForEach(async s =>
            {
                if (clients.ContainsKey(s))
                    await clients[s].SendAsync("newMessage", message, chat.Id);
            });

            //myHub.Clients.All.SendAsync("newMessage", message);
        }

        static Dictionary<int, IClientProxy> clients = new();
        public void Register(int idUser)
        {
            if (clients.ContainsKey(idUser))
                clients[idUser] = Clients.Caller;
            else
                clients.Add(idUser, Clients.Caller);
        }
    }
}
