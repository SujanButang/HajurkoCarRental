﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HajurkoCarRental.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Data;
using HajurkoCarRental.Models;

namespace HajurkoCarRental.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly HajurkoCarRentalContext _context;
        private readonly SignInManager<HajurkoCarRentalUser> _signInManager;
        private readonly UserManager<HajurkoCarRentalUser> _userManager;
        private readonly IUserStore<HajurkoCarRentalUser> _userStore;
        private readonly IUserEmailStore<HajurkoCarRentalUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHost;


        public RegisterModel(
            UserManager<HajurkoCarRentalUser> userManager,
            IUserStore<HajurkoCarRentalUser> userStore,
            SignInManager<HajurkoCarRentalUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            HajurkoCarRentalContext context,
            IWebHostEnvironment webHost)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _webHost = webHost;
        }



        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 

            [Required]
            [Display(Name = "Name")]
            [DataType(DataType.Text)]
            public string Name { get; set; }

            [Required]
            [StringLength(120)]
            [DataType(DataType.Text)]

            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "Phone")]
            [Phone]

            public string Phone { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Document")]
            [DataType(DataType.Text)]
            public string DocumentUrl { get; set; }

            [Required(ErrorMessage = "Please upload a document.")]
            public IFormFile Document { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

           
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
                var user = CreateUser();
            IFormFile doc = Input.Document;
            string uniqueFileName = GetProfilePhotoFileName(doc);
            user.DocumentUrl = uniqueFileName;
                user.Name = Input.Name;
                user.Address = Input.Address;
                user.Phone = Input.Phone;
                

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        

                        // Add the user role to the UserRoles table
                        var userRole = new IdentityUserRole<string>
                        {
                            RoleId = _context.Roles.FirstOrDefault(r => r.Name == "User").Id,
                            UserId = user.Id
                        };

                        await _context.UserRoles.AddAsync(userRole);
                        await _context.SaveChangesAsync();
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                       

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                       
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private HajurkoCarRentalUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<HajurkoCarRentalUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(HajurkoCarRentalUser)}'. " +
                    $"Ensure that '{nameof(HajurkoCarRentalUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<HajurkoCarRentalUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<HajurkoCarRentalUser>)_userStore;
        }
        private string GetProfilePhotoFileName(IFormFile Input)
        {
            string uniqueFileName = null;

            if (Input != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Input.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

    }
}
