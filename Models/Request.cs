using System;
using System.Collections.Generic;

namespace ta2jir.Models;

public partial class Request
{
    public uint RequestId { get; set; }

    public DateOnly ReqDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string State { get; set; } = null!;

    public uint RenterId { get; set; }

    public uint OfferId { get; set; }

    public uint? NegotitationId { get; set; }

    public virtual ICollection<Deal> Deals { get; } = new List<Deal>();

    public virtual Negotitation? Negotitation { get; set; }

    public virtual Offer Offer { get; set; } = null!;

    public virtual User Renter { get; set; } = null!;
}
