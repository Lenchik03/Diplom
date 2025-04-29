using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectSystemAPI.DB;

public partial class Status
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
