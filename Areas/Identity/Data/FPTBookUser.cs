using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FPTBook.Models;
namespace FPTBook.Areas.Identity.Data;

// Add profile data for application users by adding properties to the FPTBookUser class
public class FPTBookUser : IdentityUser
{
    [PersonalData]
    public string ? Name { get; set; }
    [PersonalData]
    public DateTime DOB { get; set; }
    public ICollection<Order>? Orders { get; set; }
}

