using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Date
{
    public long DateId { get; set; }

    public long ExchangeId { get; set; }

    public DateOnly DateInfo { get; set; }

    public virtual Exchange Exchange { get; set; } = null!;
}
