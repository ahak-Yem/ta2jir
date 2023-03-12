using System;
using System.Collections.Generic;

namespace ta2jir.Models;

public partial class Picture
{
    public int PictureId { get; set; }

    public string? PictureName { get; set; }

    public byte[]? PictureBytes { get; set; }

    public uint OfferId { get; set; }

    public virtual Offer Offer { get; set; } = null!;
}
