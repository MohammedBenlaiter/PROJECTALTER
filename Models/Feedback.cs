using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Feedback
{
    public long FeedbackId { get; set; }

    public long UserId { get; set; }

    public string Description { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
