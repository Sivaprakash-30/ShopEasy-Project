using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopEasy.Models;

public partial class SellerReg
{
    [Required]
    public int SId { get; set; }
    [Required]
    public string? CompanyName { get; set; }
    [Required]
    public string? SellerLiscenceNo { get; set; }
    [Required]
    public string? SellerName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    [Phone]
    public string? Contact { get; set; }
}
