using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Certification
{
    public long CertificationId { get; set; }

    public long UserId { get; set; }

    public byte[] CertificationPicture { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
