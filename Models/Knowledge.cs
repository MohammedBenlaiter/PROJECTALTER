using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Knowledge
{
    public long KnowledgeId { get; set; }

    public string KnowledgeName { get; set; } = null!;

    public long UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
