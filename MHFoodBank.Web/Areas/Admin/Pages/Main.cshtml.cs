using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin")]
    //https://openidauthority.com/how-to-prevent-the-back-button-after-logout-in-asp-net-core/
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class MainModel : AdminPageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        [BindProperty(SupportsGet = true)] 
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        public VolunteerProfile Volunteer { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Position> Positions { get; set; }
        [BindProperty(SupportsGet = true)]
        public Position DefaultPosition { get; set; }
        // make supportsget = true for this will result in it not being null
        [BindProperty] 
        public int SearchedPositionId { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }
        [BindProperty]
        public int SelectedVolunteerId { get; set; }
        [BindProperty]
        public string StatusMessage { get; set; }

        public MainModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IMapper mapper, FoodBankContext context, string currentPage = "Volunteers") : base(context, currentPage)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task OnGet(string statusMessage)
        {
            StatusMessage = statusMessage;
            var volunteerDomainModels = await PrepareModel();
            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);
        }

        public async Task OnPostSearch()
        {
            var searchedPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == SearchedPositionId);
            var volunteerDomainModels = await PrepareModel();
            Searcher searcher = new Searcher(_context);
            volunteerDomainModels = searcher.FilterVolunteersBySearch(volunteerDomainModels, SearchedName, searchedPosition);
            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);
        }
        public async Task<IActionResult> OnPostDeleteVolunteer()
        {
            Volunteer = await _context.VolunteerProfiles
                .Include(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.Id == SelectedVolunteerId);

            _context.Update(Volunteer);

            foreach(Shift shift in Volunteer.Shifts)
            {
                _context.Update(shift);
                shift.Volunteer = null;
                shift.CreateDescription();
            }

            Volunteer.Deleted = true;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = "You have successfully deleted the selected volunteer." });
        }

        private async Task<List<VolunteerProfile>> PrepareModel()
        {
            // get only volunteers
            var volunteersDomainProfiles = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null && v.Deleted == false).ToListAsync();
            Positions = await _context.Positions.ToListAsync();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");

            return volunteersDomainProfiles;
        }
    }
}