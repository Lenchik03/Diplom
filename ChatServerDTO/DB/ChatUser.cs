using System;
using System.Collections.Generic;

namespace ProjectSystemAPI.DB;

public partial class ChatUser
{
    public int Id { get; set; }

    public int IdChat { get; set; }

    public int IdUser { get; set; }

    public virtual Chat IdChatNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
