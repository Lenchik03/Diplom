using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.Model
{
    public class Message
    {
        public int Id { get; set; }

        public int IdChat { get; set; }

        public string Text { get; set; } = null!;

        public int IdSender { get; set; }

        public bool? IsReadIt { get; set; }

        public virtual Chat IdChatNavigation { get; set; } = null!;

        public virtual User IdSenderNavigation { get; set; } = null!;
    }
}
