using System;
using System.Collections.Generic;

namespace ta2jir.Models;

public partial class Negotitation
{
    public uint NegotitationId { get; set; }

    /// <summary>
    /// last price offered from owner
    /// </summary>
    public double OwnerPrice { get; set; }

    /// <summary>
    /// last price offered from renter
    /// </summary>
    public double RenterPrice { get; set; }

    /// <summary>
    /// price agreed on
    /// </summary>
    public double? LastPrice { get; set; }

    public DateOnly Date { get; set; }

    public string State { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; } = new List<Request>();
}
