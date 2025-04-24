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

    public virtual Item? Item { get; set; }
    public virtual User? User { get; set; }
}

public static class ClaimMetadata
{
    public static class Status
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string DefaultStatus = Pending;
    };
}