using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Email
{
    public long EmailId { get; set; }

    public long UserId { get; set; }

    public string EmailAdresse { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
