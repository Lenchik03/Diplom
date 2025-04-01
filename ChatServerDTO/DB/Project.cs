using System;
using System.Collections.Generic;

namespace ProjectSystemAPI.DB;

public partial class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime CompletionDate { get; set; }

    public int IdCreator { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
