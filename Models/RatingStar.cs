using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class RatingStar
{
    public long RatingId { get; set; }

    public long UserId { get; set; }

    public string Rating { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
