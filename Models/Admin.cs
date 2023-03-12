using System;
using System.Collections.Generic;

namespace ta2jir.Models;

public partial class Admin
{
    public uint AdminId { get; set; }

    public string? Name { get; set; }

    public DateOnly? DateJoined { get; set; }

    public string? Rating { get; set; }

    public uint ClassificationLevel { get; set; }
    public string Email { get; set; } = null!;

    public virtual ICollection<Case> Cases { get; } = new List<Case>();
}
