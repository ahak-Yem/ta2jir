using System;
using System.Collections.Generic;

namespace ta2jir.Models;

public partial class Case
{
    public uint CaseId { get; set; }

    public string Subject { get; set; } = null!;

    public string Report { get; set; } = null!;

    public string? Decision { get; set; }

    public string State { get; set; } = null!;

    public string IsSolved { get; set; } = null!;

    public int DealsId { get; set; }

    public uint? AdminId { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Deal Deals { get; set; } = null!;
}
