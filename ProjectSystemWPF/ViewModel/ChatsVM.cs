using ChatServerDTO.DTO;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSystemWPF.ViewModel
{
    public class ChatsVM : BaseVM
    {
        public VmCommand NewChat { get; set; }
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                FindChatAsync();
            }
        }
        public ObservableCollection<ChatDTO> Chats
        {
            get => chats;
            set { chats = value;
                Signal();
            }
        }
        public ChatDTO Chat 
        { get => chat;
            set { chat = value;
                Signal();
                if (chat != null)
                    GetMessage();
            }
        }
        public int CountMess { get; set; }
        public ObservableCollection<MessageDTO> Messages
        {
            get => messages;
            set { messages = value;
                Signal();
            }

        }

        public MessageDTO Message
        {
            get => message;
            set { message = value;
                Signal();
            }
        }
        //public UserDTO Sender
        //{
        //    get => sender;
        //    set { sender = value;
            
        //    }
        //}
        public string Text { get; set; }
        public VmCommand AttachFile { get; set; }
        public VmCommand SendMessage { get; set; }

        public ObservableCollection<ChatDTO> allChats = new ObservableCollection<ChatDTO>();
        public ObservableCollection<MessageDTO> allMessages = new ObservableCollection<MessageDTO>();
        private string searchText;
        private ChatDTO chat = new ChatDTO();
        private ObservableCollection<ChatDTO> chats = new ObservableCollection<ChatDTO>();
        private ObservableCollection<MessageDTO> messages = new ObservableCollection<MessageDTO>();
        private MessageDTO message = new MessageDTO();
        //private UserDTO sender;

        public ChatsVM()
        {
            GetChats();
            GetMessage();
            NewChat = new VmCommand(async () =>
            {
                NewMessageWindow newMessageWindow = new NewMessageWindow();
                newMessageWindow.Show();
            });

            AttachFile = new VmCommand(async () =>
            {

            });

            SendMessage = new VmCommand(async () =>
            {

            });
        }

        public async void FindChatAsync()
        {
            if (SearchText.Length > 3)
            {
                string arg = JsonSerializer.Serialize(SearchText, REST.Instance.options);
                var responce = await REST.Instance.client.PutAsync($"FindChat/{ActiveUser.GetInstance().User.Id}",
                    new StringContent(arg, Encoding.UTF8, "application/json"));
                try
                {
                    responce.EnsureSuccessStatusCode();
                    Chats = await responce.Content.ReadFromJsonAsync<ObservableCollection<ChatDTO>>(REST.Instance.options);
                    //MessageBox.Show("Отдел успешно обновлен!");

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Ошибка! Обновление отдела приостановлено!");
                    return;
                }
            }


        }

        public async void GetMessage()
        {
            var result = await REST.Instance.client.GetAsync("Messages");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                allMessages = await result.Content.ReadFromJsonAsync<ObservableCollection<MessageDTO>>(REST.Instance.options);
            }
            Messages = new ObservableCollection<MessageDTO>(allMessages.Where(s => s.IdChat == Chat.Id));


            

        }

        public async void GetChats()
        {
            var result1 = await REST.Instance.client.GetAsync($"Chats/My/{ActiveUser.GetInstance().User.Id}");
            //todo not ok

            if (result1.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                Chats = await result1.Content.ReadFromJsonAsync<ObservableCollection<ChatDTO>>(REST.Instance.options);
            }
        }
    }
}
