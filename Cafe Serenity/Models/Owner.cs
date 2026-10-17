using System;
using System.Collections.Generic;

namespace Coffee_Shop.Models;

public partial class Owner
{
    public int OwnerId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }


}
