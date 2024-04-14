using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Link
{
    public long LinksId { get; set; }

    public long SkillId { get; set; }

    public string LinkInformation { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
