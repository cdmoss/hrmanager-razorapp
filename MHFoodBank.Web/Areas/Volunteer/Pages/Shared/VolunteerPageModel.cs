using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MHFoodBank.Web.Areas.Volunteer.Pages.Shared
{
    public class VolunteerPageModel : PageModel
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly FoodBankContext _context;
        public string LoggedInUser { get; set; }

        public VolunteerPageModel(UserManager<AppUser> userManager, FoodBankContext context)
        {
            _userManager = userManager;
            _context = context;
        }
    }
}
