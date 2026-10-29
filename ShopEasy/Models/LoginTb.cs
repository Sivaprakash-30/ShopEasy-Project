using System;
using System.Collections.Generic;

namespace ShopEasy.Models;

public partial class LoginTb
{
    public int LoginId { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool? TypeOfUser { get; set; }

    public int? Status { get; set; }
}
