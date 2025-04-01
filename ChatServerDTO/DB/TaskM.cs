using System;
using System.Collections.Generic;

namespace ProjectSystemAPI.DB;

public partial class TaskM
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int IdProject { get; set; }

    public int IdExecutor { get; set; }

    public int IdStatus { get; set; }

    public int IdCreator { get; set; }

    public virtual Project IdProjectNavigation { get; set; } = null!;

    public virtual Status IdStatusNavigation { get; set; } = null!;
}
