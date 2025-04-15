using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectSystemAPI.DB;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int IdProject { get; set; }

    public int IdStatus { get; set; }

    public int IdCreator { get; set; }

    public virtual User IdCreatorNavigation { get; set; } = null!;
 
    public virtual Project IdProjectNavigation { get; set; } = null!;

    public virtual Status IdStatusNavigation { get; set; } = null!;

    public virtual ICollection<TaskForUser> TaskForUsers { get; set; } = new List<TaskForUser>();
}
