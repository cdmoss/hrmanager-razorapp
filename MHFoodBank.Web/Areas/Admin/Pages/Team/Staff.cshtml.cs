using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages.Teams
{
    [Authorize(Roles = "Staff, Admin")]
    //https://openidauthority.com/how-to-prevent-the-back-button-after-logout-in-asp-net-core/
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class StaffModel : AdminPageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        [BindProperty]
        public bool ArchivedFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        public VolunteerProfile Staff { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Position> Positions { get; set; }
        // make supportsget = true for this will result in it not being null
        [BindProperty]
        public int SearchedPositionId { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }
        [BindProperty]
        public int SelectedVolunteerId { get; set; }
        [BindProperty]
        public string StatusMessage { get; set; }
        [BindProperty]
        public StaffRegisterDto NewStaff { get; set; } = new StaffRegisterDto();

        public StaffModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IMapper mapper, FoodBankContext context, string currentPage = "Volunteers") : base(context, currentPage)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task OnGet(string statusMessage,
            bool archivedFilter = false)
        {
            StatusMessage = statusMessage;
            var volunteerDomainModels = await PrepareModel();

            var newList = new List<VolunteerProfile>();

            StatusMessage = statusMessage;

            ArchivedFilter = archivedFilter;

            foreach (var volunteer in volunteerDomainModels)
            {
                bool isActive = (volunteer.ApprovalStatus == ApprovalStatus.Approved);
                bool passeddeleted = (volunteer.ApprovalStatus == ApprovalStatus.Archived) == ArchivedFilter && ArchivedFilter;

                if (isActive || passeddeleted)
                {
                    newList.Add(volunteer);
                }
            }

            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(newList);
        }

        public async Task OnPost()
        {
            await OnGet("", ArchivedFilter);
        }

        public async Task OnPostSearch()
        {
            var volunteerDomainModels = await PrepareModel();
            Searcher searcher = new Searcher(_context);
            volunteerDomainModels = searcher.FilterVolunteersBySearch(volunteerDomainModels, SearchedName, null);
            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);
        }

        public async Task<IActionResult> OnPostChangeStatus(int volId, int status, bool archivedFilter)
        {
            Staff = await _context.VolunteerProfiles
                .Include(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.Id == volId);

            _context.Update(Staff);

            //foreach (Shift shift in Staff.Shifts)
            //{
            //    _context.Update(shift);
            //    shift.Volunteer = null;
            //    shift.CreateDescription();
            //}

            Staff.ApprovalStatus = (ApprovalStatus)status;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You have successfully changed {Staff.FirstName} {Staff.LastName} to {Enum.GetName(typeof(ApprovalStatus), status)}.", archivedFilter });
        }
        public async Task<IActionResult> OnPostDeleteStaff()
        {
            Staff = await _context.VolunteerProfiles
                .Include(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.Id == SelectedVolunteerId);

            _context.Remove(Staff);

            //foreach (Shift shift in Volunteer.Shifts)
            //{
            //    _context.Update(shift);
            //    shift.Volunteer = null;
            //    shift.CreateDescription();
            //}
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You have successfully deleted {Staff.FirstName} {Staff.LastName} from staff." });
        }

        public async Task<IActionResult> OnPostAddNewStaff()
        {
            var newStaffProfile = _mapper.Map<VolunteerProfile>(NewStaff);
            newStaffProfile.IsStaff = true;

            var user = new AppUser
            {
                UserName = NewStaff.Email,
                Email = NewStaff.Email,
                VolunteerProfile = newStaffProfile
            };

            var accountCreationResult = await _userManager.CreateAsync(user, NewStaff.Password);

            if (accountCreationResult.Succeeded)
            {
                return RedirectToPage(new { statusMessage = $"You have successfully added {newStaffProfile.FirstName} {newStaffProfile.LastName}" });
            }
            else
            {
                return RedirectToPage(new { statusMessage = $"Something went wrong when adding a the new staff member. Please try again or consult the admin." });
            }
        }

        private async Task<List<VolunteerProfile>> PrepareModel()
        {
            // get only volunteers
            var volunteersDomainProfiles = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null && v.IsStaff).ToListAsync();
            Positions = await _context.Positions.Where(p => !p.Deleted).ToListAsync();
            SearchedPositionId = Positions.FirstOrDefault(p => p.Name == "All").Id;

            return volunteersDomainProfiles;
        }
    }
}
