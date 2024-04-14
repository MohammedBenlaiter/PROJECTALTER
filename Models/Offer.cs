using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Offer
{
    public long OfferId { get; set; }

    public long UserId { get; set; }

    public long RequestId { get; set; }

    public string OfferInfo { get; set; } = null!;

    public DateOnly Deadline { get; set; }

    public long Price { get; set; }

    public virtual Request Request { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
