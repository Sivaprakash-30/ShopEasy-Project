using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEasy.Models;

public partial class CustomerReg
{
    [Required]
    public int RegId { get; set; }
    [Required]
    public string? UserName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [NotMapped]
    public Addresses? Addresses { get; set; }
    [NotMapped]
    public LoginTb? loginModel { get; set; }



}
