using System;
using System.Collections.Generic;

namespace ProjectSystemAPI.DB;

public partial class Chat
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
