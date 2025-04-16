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
        public ObservableCollection<Chat> Chats { get; set; } = new ObservableCollection<Chat>();
        public Chat Chat { get; set; } = new Chat();
        public int CountMess { get; set; }
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        public Message Message { get; set; } = new Message();
        public UserDTO Sender { get; set; }
        public string Text { get; set; }
        public VmCommand AttachFile { get; set; }
        public VmCommand SendMessage { get; set; }

        public ObservableCollection<Chat> allChats = new ObservableCollection<Chat>();
        public ObservableCollection<Message> allMessages = new ObservableCollection<Message>();
        private string searchText;

        public ChatsVM()
        {
            GetLists();
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
                    Chats = await responce.Content.ReadFromJsonAsync<ObservableCollection<Chat>>(REST.Instance.options);
                    //MessageBox.Show("Отдел успешно обновлен!");

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Ошибка! Обновление отдела приостановлено!");
                    return;
                }
            }


        }

        public async void GetLists()
        {
            var result = await REST.Instance.client.GetAsync("Messages");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                allMessages = await result.Content.ReadFromJsonAsync<ObservableCollection<Message>>(REST.Instance.options);
            }
            Messages = new ObservableCollection<Message>(allMessages.Where(s => s.IdChat == Chat.Id));


            var result1 = await REST.Instance.client.GetAsync($"Chats/My/{ActiveUser.GetInstance().User.Id}");
            //todo not ok

            if (result1.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                Chats = await result1.Content.ReadFromJsonAsync<ObservableCollection<Chat>>(REST.Instance.options);
            }

        }
    }
}
