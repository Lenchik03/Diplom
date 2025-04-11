using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.ViewModel
{
    public class ChatsVM: BaseVM
    {
        public VmCommand NewChat {  get; set; }
        public string SearchText { get; set; }
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

        public ChatsVM()
        {
            NewChat = new VmCommand(async () =>
            {

            });

            AttachFile = new VmCommand(async () =>
            {

            });

            SendMessage = new VmCommand(async () =>
            {

            });
        }

        public async void GetLists()
        {

        }
    }
}
