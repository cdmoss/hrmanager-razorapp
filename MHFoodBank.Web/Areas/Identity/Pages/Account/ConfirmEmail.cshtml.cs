using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using MHFoodBank.Common;
using MHFoodBank.Web.Services;

namespace MHFoodBank.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailConfirm _emailConfirm;

        public ConfirmEmailModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailConfirm emailConfirm)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailConfirm = emailConfirm;
        }
        [BindProperty]
        public string StatusMessage { get; set; }
        [BindProperty]
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            UserId = Convert.ToInt32(userId);
            if (userId == null || code == null)
            {
                return RedirectToPage(new { StatusMessage });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "You have successfully confirmed your email, you are now able to sign in to your account" : "Error: We could not confirm your email. Try again, or contact an administrator if the problem persists.";
            if(result.Succeeded)
            {
                return RedirectToPage("/Account/Login", new { StatusMessage, area = "Identity"});
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResendEmail(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            await _emailConfirm.TrySendConfirmationEmail(user, this.Request, this.Url);
            return RedirectToPage("/Account/Login", new { statusMessage = "We have sent you another confirmation email. Please check your inbox or spam/junk folder." });
        }
    }
}
