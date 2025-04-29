using System;
using System.Collections.Generic;

namespace ChatServerDTO.DB;

public partial class TaskForUser
{
    public int Id { get; set; }

    public int IdTask { get; set; }

    public int IdUser { get; set; }

    public virtual Task IdTaskNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
