using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Language
{
    public long LanguageId { get; set; }

    public long SkillId { get; set; }

    public string LanguageName { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
