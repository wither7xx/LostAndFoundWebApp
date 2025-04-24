using System;
using System.Collections.Generic;

namespace LostAndFoundWebApp.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string Campus { get; set; } = null!;

    public DateTime Time { get; set; }

    public string Description { get; set; } = null!;

    public string ContactInfo { get; set; } = null!;

    public string Status { get; set; } = null!;

    public bool? IsValid { get; set; }

    public string Category { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual User User { get; set; } = null!;
}

public static class ItemMetadata
{
    public static class Status
    {
        public const string Lost = "Lost";
        public const string Found = "Found";
        public const string DefaultStatus = Lost;
    };
    public static class Campus
    {
        public const string Unknown = "";
        public const string DefaultCampus = Unknown;
        public readonly static string[] CampusList = [
            Unknown,
            "campus1",
            "campus2",
            "campus3",
            "campus4",
            "campus5",
            "campus6"
        ];
    };
    public static class Category
    {
        public const string Unknown = "";
        public const string DefaultCategory = Unknown;
        public readonly static string[] CategoryList = [
            Unknown,
            "electronics",
            "documents",
            "dailyitems"
        ];
    };
}
