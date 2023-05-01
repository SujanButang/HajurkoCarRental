using HajurkoCarRental.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Areas.Identity.Data;
using HajurkoCarRental.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HajurkoCarRental.Controllers
{
    public class UserController : Controller
    {
        private readonly HajurkoCarRentalContext _context;
        private readonly SignInManager<HajurkoCarRentalUser> _signInManager;
        private readonly UserManager<HajurkoCarRentalUser> _userManager;
        private readonly IUserStore<HajurkoCarRentalUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;


        public UserController(HajurkoCarRentalContext context, SignInManager<HajurkoCarRentalUser> signInManager, UserManager<HajurkoCarRentalUser> userManager
            ,IUserStore<HajurkoCarRentalUser> userStore, ILogger<RegisterModel> logger, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
        _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Users()

        {
            if (User.IsInRole("Admin"))
            {

            var usersWithRoles = (from user in _context.Users
                                  select new
                                  {
                                      user.Id,
                                      user.Name,
                                      user.Email,
                                      user.Phone,
                                      user.Address,
                                      RoleName = (from userRole in _context.UserRoles
                                                  join role in _context.Roles on userRole.RoleId
                                                  equals role.Id
                                                  where userRole.UserId == user.Id
                                                  select role.Name).FirstOrDefault()
                                  }).ToList().Select(p => new UserRolesViewModel()
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Email = p.Email,
                                      Phone = p.Phone,
                                      Address = p.Address,
                                      RoleName = p.RoleName
                                  });
                var rentalHistory = await _context.Sales.Include(s => s.Order).ThenInclude(o=>o.Users).ToListAsync();
                ViewData["RentalHistory"] = rentalHistory;
            return View(usersWithRoles);
            }
            else
            {
                var usersWithRoles = (from user in _context.Users
                                      join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                      join role in _context.Roles on userRole.RoleId equals role.Id
                                      where role.Name == "User" || role.Name == "RegularCustomer"
                                      select new UserRolesViewModel
                                      {
                                          Id = user.Id,
                                          Name = user.Name,
                                          Email = user.Email,
                                          Phone = user.Phone,
                                          Address = user.Address,
                                          RoleName = role.Name
                                      }).ToList();
                var rentalHistory = await _context.Sales.Include(s => s.Order).ThenInclude(o => o.Users).ToListAsync();
                ViewData["RentalHistory"] = rentalHistory;

                return View(usersWithRoles);

            }
        }

        //public IActionResult Create()
        //{
        //    var model = new RegisterModel(
        //         _userManager,
        //    _userStore ,
        //    _signInManager ,
        //    _logger ,
        //    _emailSender 
        //        );
        //    return View(model);
        //}

        public async Task<IActionResult> ChangeRole(string id)
        {
            if(id==null || _context.Users == null)
            {
                return NotFound();
            }
            var selectedUser = (from user in _context.Users
                                where user.Id == id
                                select new UserRolesViewModel()
                                {
                                    Id = user.Id,
                                    Name = user.Name,
                                    Email = user.Email,
                                    Phone = user.Phone,
                                    Address = user.Address,
                                    RoleId = (from userRole in _context.UserRoles
                                              where userRole.UserId == user.Id
                                              select userRole.RoleId).FirstOrDefault(),
                                    RoleName = (from userRole in _context.UserRoles
                                                join role in _context.Roles on userRole.RoleId equals role.Id
                                                where userRole.UserId == user.Id
                                                select role.Name).FirstOrDefault()
                                }).FirstOrDefault();


            var roles = new List<string>();

            if (User.IsInRole("Admin"))
            {
                // Get all roles if the user is an admin
                roles = _context.Roles.Select(r => r.Name).ToList();
            }
            else
            {
                // Otherwise, only include "User" and "RegularCustomer"
                roles = new List<string> { "User", "RegularCustomer" };
            }

            // Create a SelectList using the modified list of role names, with the name as both the value and the text to display
            var rolesSelectList = new SelectList(roles);

            // Store the SelectList in ViewData so it can be accessed in the view
            ViewData["RolesSelectList"] = rolesSelectList;

            return View(selectedUser);

        }

        [HttpPost]
        public async Task<IActionResult> RoleChange(string userId, string RoleName)
        {
            {
                var user = await _userManager.FindByIdAsync(userId);
                var currentRole = await _userManager.GetRolesAsync(user);

                // Remove the user from their current role
                await _userManager.RemoveFromRoleAsync(user, currentRole[0]);

                // Add the user to the new role
                await _userManager.AddToRoleAsync(user, RoleName);

                // Redirect to the user list page
                return RedirectToAction(nameof(Users));
            }
        }


        public async Task<IActionResult> Delete(string? id)
        {
            if(id==null || _context.Users == null)
            {
                return NotFound();
            }
            var selectedUser = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (selectedUser == null)
            {
                return NotFound();
            }
            return View(selectedUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set null");
            }
            var selecteduser = await _context.Users.FindAsync(id);
            if (selecteduser != null)
            {
                _context.Users.Remove(selecteduser);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }
    }
}
