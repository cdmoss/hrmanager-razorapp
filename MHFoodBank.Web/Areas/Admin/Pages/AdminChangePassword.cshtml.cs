using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin")]
    public class ChangePasswordModel : AdminPageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            FoodBankContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<ChangePasswordModel> logger, string currentPage = "Change Volunteer Password") : base(context, currentPage)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty] 
        public AppUser Volunteer { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Volunteer = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == id);
            await _context.Entry(Volunteer).Reference(p => p.VolunteerProfile).LoadAsync();

            if (Volunteer == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(Volunteer);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Volunteer = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
           await _context.Entry(Volunteer).Reference(p => p.VolunteerProfile).LoadAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Volunteer == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(Volunteer, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToPage("VolunteerDetails", new { id = Volunteer.VolunteerProfile.Id, statusMessage = "You have successfully changed this volunteer's password." });
        }
    }
}
