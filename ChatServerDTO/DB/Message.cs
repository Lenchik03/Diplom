using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectSystemAPI.DB;

public partial class Message
{
    public int Id { get; set; }

    public int IdChat { get; set; }

    public string Text { get; set; } = null!;

    public int IdSender { get; set; }

    public bool? IsReadIt { get; set; }

    public byte[]? Document { get; set; }

    public string? DocumentTitle { get; set; }

    public virtual Chat IdChatNavigation { get; set; } = null!;

    public virtual User IdSenderNavigation { get; set; } = null!;
}
