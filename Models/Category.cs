using System;
using System.Collections.Generic;

namespace ta2jir.Models;

public partial class Category
{
    public int CategorieId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Offer> Offers { get; } = new List<Offer>();
}
