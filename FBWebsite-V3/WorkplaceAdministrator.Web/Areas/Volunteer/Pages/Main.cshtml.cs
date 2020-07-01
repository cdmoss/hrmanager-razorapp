using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Web.Areas.Volunteer.Pages.Shared;
using WorkplaceAdministrator.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WorkplaceAdministrator.Web.Areas.Volunteer.Pages
{
    [Authorize(Roles = "Volunteer")]
    public class MainModel : VolunteerPageModel
    {
        public MainModel(UserManager<AppUser> userManager, FoodBankContext context) : base(userManager, context)
        {

        }

        public async Task OnGet()
        {
            AppUser CurrentUser = await _userManager.GetUserAsync(User);
            LoggedInUser = CurrentUser.VolunteerProfile.FirstName + " " + CurrentUser.VolunteerProfile.LastName;
        }
    }
}