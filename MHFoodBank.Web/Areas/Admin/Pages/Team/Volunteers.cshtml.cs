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

        [BindProperty]
        public bool ApprovedFilter { get; set; }
        [BindProperty]
        public bool PendingFilter { get; set; }
        [BindProperty]
        public bool NotApprovedFilter { get; set; }
        [BindProperty]
        public bool ArchivedFilter { get; set; }
        [BindProperty(SupportsGet = true)] 
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        public VolunteerProfile Volunteer { get; set; }
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

        public MainModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IMapper mapper, FoodBankContext context, string currentPage = "Volunteers") : base(context, currentPage)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task OnGet(
            string statusMessage, 
            bool approvedFilter = true, 
            bool pendingFilter = true, 
            bool notApprovedFilter = false, 
            bool archivedFilter = false)
        {
            StatusMessage = statusMessage;
            var volunteerDomainModels = await PrepareModel();

            var newList = new List<VolunteerProfile>();

            ApprovedFilter = approvedFilter;
            PendingFilter = pendingFilter;
            NotApprovedFilter = notApprovedFilter;
            ArchivedFilter = archivedFilter;

            foreach (var volunteer in volunteerDomainModels)
            {
                bool passedapproved = (volunteer.ApprovalStatus == ApprovalStatus.Approved) == ApprovedFilter && ApprovedFilter;
                bool passedpending = (volunteer.ApprovalStatus == ApprovalStatus.Pending) == PendingFilter && PendingFilter;
                bool passednotapproved = (volunteer.ApprovalStatus == ApprovalStatus.NotApproved) == NotApprovedFilter && NotApprovedFilter;
                bool passeddeleted = (volunteer.ApprovalStatus == ApprovalStatus.Archived) == ArchivedFilter && ArchivedFilter;

                if (passedapproved || passedpending || passednotapproved || passeddeleted)
                {
                    newList.Add(volunteer);
                }
            }

            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(newList);
        }

        public async Task OnPost()
        {
            await OnGet("", ApprovedFilter, PendingFilter, NotApprovedFilter, ArchivedFilter);
        }

        public async Task<IActionResult> OnPostChangeStatus(int volId, int status, bool approvedFilter, bool pendingFilter, bool notApprovedFilter, bool archivedFilter)
        {
            var volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == volId);
            _context.Update(volunteer);
            volunteer.ApprovalStatus = (ApprovalStatus)status;
            await _context.SaveChangesAsync();

            if(volunteer.ApprovalStatus == ApprovalStatus.NotApproved)
            {
                return RedirectToPage(new { statusMessage = $"You have successfully changed the status of {volunteer.FirstName} {volunteer.LastName} to Not Approved", approvedFilter, pendingFilter, notApprovedFilter, archivedFilter });
            }

            return RedirectToPage(new { statusMessage = $"You have successfully changed the status of {volunteer.FirstName} {volunteer.LastName} to {Enum.GetName(typeof(ApprovalStatus), status)}", approvedFilter, pendingFilter, notApprovedFilter, archivedFilter });
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
            var user = await _context.Users
                .Include(p => p.VolunteerProfile).ThenInclude(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.VolunteerProfile.Id == SelectedVolunteerId);

            foreach (Shift shift in user.VolunteerProfile.Shifts)
            {
                _context.Update(shift);
                shift.Volunteer = null;
            }

            _context.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You have successfully deleted {user.VolunteerProfile.FirstName} {user.VolunteerProfile.LastName} volunteer." });
        }

        private async Task<List<VolunteerProfile>> PrepareModel()
        {
            // get only volunteers
            var volunteersDomainProfiles = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null && !v.IsStaff).ToListAsync();
            Positions = _context.Positions.Where(p => !p.Deleted).OrderBy(p => p.Name).ToList();
            SearchedPositionId = Positions.FirstOrDefault(p => p.Name == "All").Id;

            return volunteersDomainProfiles;
        }
    }
}