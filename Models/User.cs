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

public static class UserMetadata
{
    public static class Role
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string DefaultRole = User;
    };
    public static class Password
    {
        public const string PasswordPlaceholder = "000000";
    };
}