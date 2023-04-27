using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HajurkoCarRental.Areas.Identity.Data;

// Add profile data for application users by adding properties to the HajurkoCarRentalUser class
public class HajurkoCarRentalUser : IdentityUser
{
    public string Name { get; set; }

    public string Address { get; set; }

    public string Phone { get; set; }

}

