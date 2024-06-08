using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Request
{
    public long RequestId { get; set; }

    public long UserId { get; set; }

    public string RequestTitle { get; set; } = null!;

    public string RequestDescription { get; set; } = null!;

    public DateOnly Deadline { get; set; }

    public string? RequestType { get; set; }

    public string? RequestStatus { get; set; }

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual User User { get; set; } = null!;
}
