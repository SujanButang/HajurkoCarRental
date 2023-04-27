using HajurkoCarRental.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Models;

namespace HajurkoCarRental.Data;

public class HajurkoCarRentalContext : IdentityDbContext<HajurkoCarRentalUser>
{
    public HajurkoCarRentalContext(DbContextOptions<HajurkoCarRentalContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<HajurkoCarRental.Models.Car>? Car { get; set; }
    public DbSet<HajurkoCarRental.Models.Order>? Order { get; set; }
    public DbSet<HajurkoCarRental.Models.Sales>? Sales { get; set; }

}
