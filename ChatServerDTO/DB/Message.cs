using System;
using System.Collections.Generic;

namespace ChatServerDTO.DB;

public partial class Message
{
    public int Id { get; set; }

    public int IdChat { get; set; }

    public string Text { get; set; } = null!;

    public int IdSender { get; set; }

    public byte[]? Document { get; set; }

    public string? DocumentTitle { get; set; }

    public DateTime? DateOfSending { get; set; }

    public virtual Chat IdChatNavigation { get; set; } = null!;

    public virtual User IdSenderNavigation { get; set; } = null!;
}
