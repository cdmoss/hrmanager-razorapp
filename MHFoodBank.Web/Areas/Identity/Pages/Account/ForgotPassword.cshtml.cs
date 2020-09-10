using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MHFoodBank.Web.Areas.Identity.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IPasswordRecoveryService _recoveryService;
        private readonly ILogger<ForgotPasswordModel> _logger;
        [BindProperty]
        public string Email { get; set; }

        public ForgotPasswordModel(IPasswordRecoveryService recoveryService, ILogger<ForgotPasswordModel> logger)
        {
            _recoveryService = recoveryService;
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostForgotPassword()
        {
            if(ModelState.IsValid)
            {
                try
                {
                    await _recoveryService.SendForgotPasswordEmailAsync(Email, Request, Url);
                    return RedirectToPage("/Account/Login", new { statusMessage = "You should receive an email with instructions on changing your password. Make sure you check the spam/junk folder if the email is not in your inbox. Please contact your supervisor if you did not receive an email, or are not able to change your password." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }
            }

            return Page();
        }
    }
}
