using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Data;
using HajurkoCarRental.Areas.Identity.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HajurkoCarRentalContextConnection") ?? throw new InvalidOperationException("Connection string 'HajurkoCarRentalContextConnection' not found.");

builder.Services.AddDbContext<HajurkoCarRentalContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<HajurkoCarRentalUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<HajurkoCarRentalContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdministratorOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();




app.Run();
