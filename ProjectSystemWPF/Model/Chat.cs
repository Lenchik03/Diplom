using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.Model
{
    public class Chat
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
