using System;
using System.Collections.Generic;

namespace ChatServerDTO.DB;

public partial class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime CompletionDate { get; set; }

    public int IdCreator { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual User IdCreatorNavigation { get; set; } = null!;

    public virtual ICollection<ChatServerDTO.DB.Task> Tasks { get; set; } = new List<ChatServerDTO.DB.Task>();
}
