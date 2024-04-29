using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Skill
{
    public long SkillId { get; set; }

    public long UserId { get; set; }

    public string SkillName { get; set; } = null!;

    public string SkillDescription { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public string SkillLevel { get; set; } = null!;

    public string SkillType { get; set; } = null!;

    public virtual ICollection<Exchange> ExchangeSkillReceives { get; set; } = new List<Exchange>();

    public virtual ICollection<Exchange> ExchangeSkillSends { get; set; } = new List<Exchange>();

    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();

    public virtual ICollection<Link> Links { get; set; } = new List<Link>();

    public virtual User User { get; set; } = null!;
}
