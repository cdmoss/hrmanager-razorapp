using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Web.Areas.Admin.Pages.Shared;
using WorkplaceAdministrator.Web.Data;
using WorkplaceAdministrator.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WorkplaceAdministrator.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin")]
    //https://openidauthority.com/how-to-prevent-the-back-button-after-logout-in-asp-net-core/
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class MainModel : AdminPageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        [BindProperty(SupportsGet = true)] 
        public List<VolunteerProfile> Volunteers { get; set; }
        public VolunteerProfile Volunteer { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Position> Positions { get; set; }
        [BindProperty(SupportsGet = true)]
        public Position DefaultPosition { get; set; }
        // make supportsget = true for this will result in it not being null
        [BindProperty] 
        public Position SearchedPosition { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }

        public MainModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, FoodBankContext context) : base(context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task OnGet()
        {
            await PrepareModel();
        }

        public async Task OnPostSearch()
        {
            await PrepareModel();
            Searcher searcher = new Searcher(_context);
            Position searchedPosition = searcher.GetSearchedPosition(Request);
            Volunteers = searcher.FilterVolunteersBySearch(Volunteers, SearchedName, searchedPosition);
        }
        public async Task OnPostDeleteVolunteer()
        {
            int id = Convert.ToInt32(Request.Form["volunteer-id-modal"]);
            Volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(p => p.Id == id);
            await _context.Entry(Volunteer).Collection(p => p.Shifts).LoadAsync();

            foreach(Shift shift in Volunteer.Shifts)
            {
                shift.Volunteer = null;
                shift.CreateDescription();
            }

            _context.Remove(Volunteer);
            await _context.SaveChangesAsync();
        }

        private async Task PrepareModel()
        {
            // get only volunteers
            Volunteers = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null).ToListAsync();
            Positions = await _context.Positions.ToListAsync();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");
        }
    }
}