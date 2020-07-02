using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MHFoodBank.Web.Areas.Volunteer.Pages
{
    [BindProperties]
    public class WelcomeModel : VolunteerPageModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public WelcomeModel(UserManager<AppUser> userManager, FoodBankContext context) : base(userManager,
            context)
        {
            
        }

        public async Task OnGet()
        {
            AppUser currentUser = _userManager.GetUserAsync(User).Result;
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
            FirstName = currentUser.VolunteerProfile.FirstName;
            LastName = currentUser.VolunteerProfile.LastName;
        }
    }
}