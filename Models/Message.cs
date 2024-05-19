using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Message
{
    public long MessageId { get; set; }

    public long SenderId { get; set; }

    public long ReceiverId { get; set; }

    public string Content { get; set; } = null!;

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
