using System;
using System.Collections.Generic;

namespace LostAndFoundWebApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool? IsValid { get; set; }

    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
