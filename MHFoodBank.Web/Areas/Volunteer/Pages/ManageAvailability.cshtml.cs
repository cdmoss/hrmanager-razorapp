using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MHFoodBank.Web.Services;

namespace MHFoodBank.Web.Areas.Volunteer.Pages
{
    public class ManageAvailabilityModel : VolunteerPageModel
    {
        [BindProperty]
        public Dictionary<string, List<Availability>> OldAvailability { get; set; } = new Dictionary<string, List<Availability>>();
        [BindProperty]
        public string StatusMessage { get; set; }

        public ManageAvailabilityModel(FoodBankContext context, UserManager<AppUser> userManager) : base(userManager, context)
        {
        }

        public async Task<IActionResult> OnGet(string statusMessage = null)
        {
            StatusMessage = statusMessage;
            AppUser currentUser = _userManager.GetUserAsync(User).Result;
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(currentUser.VolunteerProfile).Collection(p => p.Availabilities).LoadAsync();
            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
            AvailabilityHandler availability = new AvailabilityHandler();
            OldAvailability = availability.GetSortedAvailability(currentUser.VolunteerProfile.Availabilities);

            return Page();
        }

        public async Task<IActionResult> OnPostSaveTimesAsync(IFormCollection formData)
        {
            AppUser currentUser = _userManager.GetUserAsync(User).Result;
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(currentUser.VolunteerProfile).Collection(p => p.Availabilities).LoadAsync();

            AvailabilityHandler availability = new AvailabilityHandler();
            await availability.SetVolunteerAvailability(formData, currentUser.VolunteerProfile, _context);

            return RedirectToPage(new { statusMessage = "You have successfully changed your availability" });
        }
    }
}