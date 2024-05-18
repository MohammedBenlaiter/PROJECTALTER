using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class Wishlist
{
    public long WishlistId { get; set; }

    public string WishlistName { get; set; } = null!;

    public long UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
