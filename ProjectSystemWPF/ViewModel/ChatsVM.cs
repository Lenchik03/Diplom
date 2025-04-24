using ChatServerDTO.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using static System.Net.Mime.MediaTypeNames;

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
        HubConnection _connection = SignalR.Instance.CreateConnection();
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
            HubMethods();
            NewChat = new VmCommand(async () =>
            {
                NewMessageWindow newMessageWindow = new NewMessageWindow(Chat);
                newMessageWindow.Show();
            });

            AttachFile = new VmCommand(async () =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog();
                if(openFileDialog.ShowDialog() == true)
                {
                    var filePath = openFileDialog.FileName;
                    var fileName = Path.GetFileName(filePath);
                    var fileContent = await File.ReadAllBytesAsync(filePath);
                    Message.DocumentTitle = fileName;
                    Message.Document = fileContent;
                }
                
            });

            SendMessage = new VmCommand(async () =>
            {
                Message.Sender = ActiveUser.GetInstance().User;
                Message.IdSender = ActiveUser.GetInstance().User.Id;
                Message.IdChat = Chat.Id;
                Message.Chat = Chat;
                
                await _connection.SendAsync("NewMessage", ActiveUser.GetInstance().User, Message, Chat);
            });
        }

        private void HubMethods()
        {
            _connection.On<string, int>("newMessage", (mess, chatId) =>
            {
                var chat = Chats.FirstOrDefault(c => c.Id == chatId);
                Notifier notifier = new Notifier(cfg =>
                {
                    cfg.PositionProvider = new WindowPositionProvider(
                        parentWindow: System.Windows.Application.Current.MainWindow,
                        corner: Corner.TopRight,
                        offsetX: 10,
                        offsetY: 10);

                    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                        notificationLifetime: TimeSpan.FromSeconds(3),
                        maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                    cfg.Dispatcher = System.Windows.Application.Current.Dispatcher;
                });
                
                //var notify = new ToastContentBuilder();
                //notify.AddText("Новое сообщение!");
                //notify.AddArgument($"Вам пришло новое сообщение в чате {chat.Title}");
                //notify.AddArgument(mess);
                //notify.GetToastContent();
                //var toast = notify.GetToastContent();
                notifier.ShowInformation($"Вам пришло новое сообщение в чате {chat.Title}");


            });
        }




        public async void FindChatAsync()
        {
            if (SearchText.Length > 3)
            {
                string arg = JsonSerializer.Serialize(SearchText, REST.Instance.options);
                var responce = await REST.Instance.client.PostAsync($"Chats/FindChat/{ActiveUser.GetInstance().User.Id}",
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

        internal void Select(ChatDTO chat)
        {

            NewMessageWindow newMessageWindow = new NewMessageWindow(chat);
            newMessageWindow.ShowDialog();
        }
    }
}
