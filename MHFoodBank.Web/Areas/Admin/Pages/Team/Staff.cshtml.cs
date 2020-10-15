using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages.Teams
{
    [Authorize(Roles = "Staff, Admin")]
    public class StaffModel : AdminPageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        [BindProperty]
        public bool ArchivedFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<VolunteerMinimalDto> Staff { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Position> Positions { get; set; }
        // make supportsget = true for this will result in it not being null
        [BindProperty]
        public int SearchedPositionId { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }
        [BindProperty]
        public int SelectedStaffId { get; set; }
        [BindProperty]
        public string StatusMessage { get; set; }
        [BindProperty]
        public StaffRegisterDto NewStaff { get; set; } = new StaffRegisterDto();

        public StaffModel(UserManager<AppUser> userManager, IMapper mapper, FoodBankContext context, string currentPage = "Staff") : base(context, currentPage)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task OnGet(string statusMessage = null, bool archivedFilter = false)
        {
            StatusMessage = statusMessage;
            var volunteerDomainModels = await PrepareModel(statusMessage, archivedFilter);
            Staff = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);
        }

        public async Task OnPost()
        {
            await OnGet("", ArchivedFilter);
        }

        public async Task OnPostSearch()
        {
            var volunteerDomainModels = await PrepareModel();
            Searcher searcher = new Searcher();
            volunteerDomainModels = searcher.FilterVolunteersBySearch(volunteerDomainModels, SearchedName, null);
            Staff = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);

            ArchivedFilter = ArchivedFilter;
        }

        public async Task<IActionResult> OnPostChangeStatus(int volId, int status, bool archivedFilter)
        {
            var selectedStaff = await _context.VolunteerProfiles
                .Include(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.Id == volId);

            _context.Update(selectedStaff);

            selectedStaff.ApprovalStatus = (ApprovalStatus)status;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You have successfully changed {selectedStaff.FirstName} {selectedStaff.LastName} to {Enum.GetName(typeof(ApprovalStatus), status)}.", archivedFilter });
        }
        public async Task<IActionResult> OnPostDeleteStaff()
        {
            var selectedStaff = await _context.VolunteerProfiles
                .Include(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.Id == SelectedStaffId);

            _context.Remove(selectedStaff);

            //foreach (Shift shift in Volunteer.Shifts)
            //{
            //    _context.Update(shift);
            //    shift.Volunteer = null;
            //    shift.CreateDescription();
            //}
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You have successfully deleted {selectedStaff.FirstName} {selectedStaff.LastName} from staff." });
        }

        public async Task<IActionResult> OnPostAddNewStaff()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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
                return RedirectToPage(new { statusMessage = $"Error: Something went wrong when adding the new staff member. Please try again or consult the admin." });
            }
        }

        private async Task<List<VolunteerProfile>> PrepareModel(string statusMessage = null, bool archiveFilter = false)
        {
            // get only staff
            var staffDomainModels = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null && v.IsStaff).ToListAsync();
            Positions = _context.Positions.Where(p => !p.Deleted).OrderBy(p => p.Name).ToList();
            SearchedPositionId = Positions.FirstOrDefault(p => p.Name == "All").Id;

            var filteredStaff = new List<VolunteerProfile>();

            StatusMessage = statusMessage;

            ArchivedFilter = archiveFilter;

            // filter according to parameters
            foreach (var volunteer in staffDomainModels)
            {
                bool isActive = (volunteer.ApprovalStatus == ApprovalStatus.Approved);
                bool passeddeleted = (volunteer.ApprovalStatus == ApprovalStatus.Archived) == ArchivedFilter && ArchivedFilter;

                if (isActive || passeddeleted)
                {
                    filteredStaff.Add(volunteer);
                }
            }

            return filteredStaff;
        }
    }
}
