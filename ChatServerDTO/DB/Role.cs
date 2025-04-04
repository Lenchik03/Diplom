using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectSystemAPI.DB;

public partial class Role
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
