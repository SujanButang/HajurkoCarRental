using HajurkoCarRental.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Areas.Identity.Data;
using HajurkoCarRental.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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
            return View(usersWithRoles);

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
