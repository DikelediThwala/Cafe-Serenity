using System;
using System.Collections.Generic;

namespace Coffee_Shop.Models;

public partial class BusinessReview
{
    public int ReviewId { get; set; }

    public int? OwnerId { get; set; }

    public int Rating { get; set; }

    public string? ReviewText { get; set; }

    public DateTime? CreatedAt { get; set; }
}
