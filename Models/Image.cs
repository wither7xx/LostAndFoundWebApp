using System;
using System.Collections.Generic;

namespace LostAndFoundWebApp.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public string ImagePath { get; set; } = null!;

    public int ItemId { get; set; }

    public virtual Item Item { get; set; } = null!;
}
