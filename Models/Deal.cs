using System;
using System.Collections.Generic;

namespace ta2jir.Models;

public partial class Deal
{
    public int DealsId { get; set; }

    public string State { get; set; } = null!;

    public string? OwnerComment { get; set; }

    public string? RenterComment { get; set; }

    public string? OwnerRating { get; set; }

    public string? RenterRating { get; set; }

    public uint RequestId { get; set; }

    public virtual ICollection<Case> Cases { get; } = new List<Case>();

    public virtual Request Request { get; set; } = null!;
}
