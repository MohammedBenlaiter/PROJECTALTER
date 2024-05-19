using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Token
{
    public long TokenId { get; set; }

    public long UserId { get; set; }

    public string Token1 { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
