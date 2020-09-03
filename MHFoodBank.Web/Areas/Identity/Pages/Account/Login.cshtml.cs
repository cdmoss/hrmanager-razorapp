using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MHFoodBank.Common;

namespace MHFoodBank.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            ILogger<LoginModel> logger,
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string LoginError { get; set; }
        [BindProperty]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task OnGetAsync(string statusMessage = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = statusMessage;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                try
                {
                    return await TryLogin();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    ModelState.AddModelError(string.Empty, "Something went wrong with the login process, please try again or contact an administrator.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<IActionResult> TryLogin()
        {
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await RedirectUserBasedOnRole();
            }

            ModelState.AddModelError(String.Empty, "Invalid login attempt. Make sure you've confirmed your email. (Check your spam/junk folder)");
            return Page();
        }
        private async Task<bool> CheckIfUserIsVolunteer(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.Any(r => r == "Volunteer");
        }

        private async Task<IActionResult> RedirectUserBasedOnRole()
        {
            var user = await _userManager.FindByNameAsync(Input.Email);
            bool UserIsVolunteer = await CheckIfUserIsVolunteer(user);

            if (UserIsVolunteer)
            {
                return RedirectToPage("/Welcome", new { area = "Volunteer" });
            }
            else
            {
                return RedirectToPage("/Team/Volunteers", new { area = "Admin" });
            }
        }

    }
}
