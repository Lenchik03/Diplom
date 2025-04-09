using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectSystemAPI.DB;

public partial class Chat
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
