using System;
using System.Collections.Generic;

namespace ta2jir.Models;

public partial class Offer
{
    public uint OfferId { get; set; }

    public string ObjName { get; set; } = null!;

    public string ObjDescription { get; set; } = null!;

    public double Price { get; set; }

    public double Deposit { get; set; }

    public DateOnly DatePosted { get; set; }

    public uint Quantity { get; set; }

    public DateOnly AvailableFrom { get; set; }

    public DateOnly? AvailableTo { get; set; }

    public string State { get; set; } = null!;

    public string? Defects { get; set; }

    public string? OtherDetails { get; set; }

    public int CategorieId { get; set; }

    public uint OwnerId { get; set; }

    public virtual Category Categorie { get; set; } = null!;

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Picture> Pictures { get; } = new List<Picture>();

    public virtual ICollection<Request> Requests { get; } = new List<Request>();
}
