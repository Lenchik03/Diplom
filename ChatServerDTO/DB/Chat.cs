using ProjectSystemAPI.DB;
using System;
using System.Collections.Generic;

namespace ChatServerDTO.DB;

public partial class Chat
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public byte[]? ImagePath { get; set; }

    public int? IdCreator { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

    public virtual User? IdCreatorNavigation { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
