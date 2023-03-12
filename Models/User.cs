using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ta2jir.Models;

public partial class User
{
    public uint UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public DateTime? Birthdate { get; set; }

    public double? UserRating { get; set; }

    public DateTime DateJoined { get; set; }

    public string IsBlocked { get; set; } = null!;
    
    public string? ProfilePic { get; set; }
    [NotMapped]
    public IFormFile? ProfilePicFile { get; set; }

    public string Password { get; set; } = null!;

    [NotMapped]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [NotMapped]
    [Compare("NewPassword")]
    [DataType(DataType.Password)]
    public string? RepeatPassword { get; set; }

    public virtual ICollection<Offer> Offers { get; } = new List<Offer>();

    public virtual ICollection<Request> Requests { get; } = new List<Request>();
}
