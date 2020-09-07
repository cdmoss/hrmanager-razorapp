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
using MHFoodBank.Web.Services;

namespace MHFoodBank.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailConfirm _emailConfirm;

        public LoginModel(SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            ILogger<LoginModel> logger,
            UserManager<AppUser> userManager,
            IEmailConfirm emailConfirm)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailConfirm = emailConfirm;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string LoginError { get; set; }
        [BindProperty]
        public int RegisteredUserId { get; set; }
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task OnGetAsync(int registeredUserId = 0, string statusMessage = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            RegisteredUserId = registeredUserId;
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

        public async Task<IActionResult> OnPostResendEmail(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            string emailResendResult = "";
            if (user != null)
            {   
                try
                {
                    await _emailConfirm.TrySendConfirmationEmail(user, Request, Url);
                    emailResendResult = "We have resent the confirmation email. Remember to check your spam folder. If you still do not recieve the email, please contact your supervisor.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    emailResendResult = "Error: Something went wrong when attempting to send the confirmation email, please contact your supervisor.";
                }

                return RedirectToPage("/Account/Login", new { statusMessage = emailResendResult });
            }
            else
            {
                emailResendResult = "Error: Something went wrong when attempting to send the confirmation email, please contact your supervisor.";
                return RedirectToPage("/Account/Login", new { statusMessage = emailResendResult });
            }
        }

        private async Task<IActionResult> TryLogin()
        {
            var user = await _userManager.FindByNameAsync(Input.Email);
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Volunteer"))
            {
                bool allowed = await _userManager.IsEmailConfirmedAsync(user);
                if (!allowed)
                {
                    ModelState.AddModelError(String.Empty, "Invalid login attempt. Make sure you've confirmed your email. (Check your spam/junk folder)");
                    return Page();
                }
            }
            try
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, false, false);
                if (result.Succeeded)
                {
                    return await RedirectUserBasedOnRole(userRoles);
                }

                ModelState.AddModelError(String.Empty, "Invalid login attempt. Make sure you entered the right credentials.");
                return Page();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<IActionResult> RedirectUserBasedOnRole(IList<string> userRoles)
        {
            if (userRoles.Contains("Volunteer"))
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
