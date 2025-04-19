using System;
using System.Collections.Generic;

namespace LostAndFoundWebApp.Models;

public partial class Claim
{
    public int ClaimId { get; set; }

    public string ClaimDescription { get; set; } = null!;

    public string ProofDocPath { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public int ItemId { get; set; }

    public int UserId { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
