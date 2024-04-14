using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Exchange
{
    public long ExchangeId { get; set; }

    public long ReciverId { get; set; }

    public long SenderId { get; set; }

    public long SkillSendId { get; set; }

    public long SkillReceiveId { get; set; }

    public string Statues { get; set; } = null!;

    public virtual ICollection<Date> Dates { get; set; } = new List<Date>();

    public virtual User Reciver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;

    public virtual Skill SkillReceive { get; set; } = null!;

    public virtual Skill SkillSend { get; set; } = null!;
}
