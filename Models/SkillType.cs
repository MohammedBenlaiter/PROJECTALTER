using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class SkillType
{
    public long TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
