using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MHFoodBank.Web.Areas.Identity.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        public string Email { get; set; }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostForgotPassword()
        {
            if(ModelState.IsValid)
            {
                return RedirectToPage("/Account/Login", new { statusMessage = "You should receive an email with instructions on changing your password. Make sure you check the spam/junk folder if the email is not in your inbox. Please contact your supervisor if you did not receive an email, or are not able to change your password." });
            }

            return Page();
        }
    }
}
