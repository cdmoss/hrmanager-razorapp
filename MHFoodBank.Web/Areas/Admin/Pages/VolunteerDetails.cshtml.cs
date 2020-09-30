using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using MHFoodBank.Common.Dtos;
using AutoMapper;
using MHFoodBank.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MHFoodBank.Web.Areas.Admin.Pages.Shared
{
    [Authorize(Roles = "Staff, Admin")]
    public class VolunteerDetailsModel : AdminPageModel
    {
        [BindProperty]
        public Dictionary<string, List<Availability>> OldAvailability { get; set; } = new Dictionary<string, List<Availability>>();
        [BindProperty]
        public VolunteerAdminReadEditDto DetailsModel { get; set; }
        [BindProperty]
        public Dictionary<string, List<Availability>> Availability { get; set; } = new Dictionary<string, List<Availability>>();
        [BindProperty]
        public int MaxAvailabilityCount { get { return GetMaxAvailabilityCount(); } }

        [BindProperty]
        public List<Position> Positions { get; set; }
        [BindProperty]
        public string StatusMessage { get; set; }
        public string ReturnUrl { get; set; }

        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public VolunteerDetailsModel(FoodBankContext context, IMapper mapper, UserManager<AppUser> userManager, string currentPage = "Volunteer Details") : base(context, currentPage)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult OnGet(int id, string statusMessage = null, string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            StatusMessage = statusMessage;
            PrepareModel(id);

            return Page();
        }

        public IActionResult OnPostAdminChangePassword(int id)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToPage("AdminChangePassword", new { id = id });
            }

            return RedirectToPage("AdminChangePassword", new { id = id });
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(int check, int id)
        {
            PrepareModel(id);
            await UpdateStatus(check);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = id });
        }

        public async Task<IActionResult> OnPostSaveChangesAsync(IFormCollection formData, int id, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var errors = ModelState.Values.Select(v => v.Errors);
            if (!ModelState.IsValid)
            {
                return OnGet(id: id, statusMessage: "Error: One or more of the fields was not filled in properly.");
            }

            // retrieve the user to be updated, load volunteer profile and all navigation properties
            AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == id);

            await _context.Entry(user).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(user.VolunteerProfile).Collection(p => p.Availabilities).LoadAsync();
            await _context.Entry(user.VolunteerProfile).Collection(p => p.References).LoadAsync();
            await _context.Entry(user.VolunteerProfile).Collection(p => p.WorkExperiences).LoadAsync();

            bool DuplicateEmailFound = await _context.Users.AnyAsync(u => u.NormalizedEmail == DetailsModel.Email.ToUpper() && u.Id != user.Id);
            if (DuplicateEmailFound)
            {
                return OnGet(id: id, statusMessage: "Error: The email you entered already belongs to someone else.");
            }

            await UpdateUserProfile(user);
            var volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(p => p.Id == id);

            AvailabilityHandler availability = new AvailabilityHandler();
            bool successfullySetAvailabilities = await availability.UpdateVolunteerAvailability(formData, volunteer, _context);

            if (!successfullySetAvailabilities)
            {
                return RedirectToPage(new { statusMessage = "Error: One of the availabilities you entered was invalid. Make sure you enter a value for both the start and end time." });
            }

            return RedirectToPage(new { statusMessage = "Successfully updated the volunteer profile." });
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var user = await _context.Users
                .Include(p => p.VolunteerProfile).ThenInclude(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.VolunteerProfile.Id == id);

            foreach (Shift shift in user.VolunteerProfile.Shifts)
            {
                _context.Update(shift);
                shift.Volunteer = null;
            }

            string redirectLocation = "";

            if (user.VolunteerProfile.IsStaff)
            {
                redirectLocation = "Team/Staff";
            }
            else
            {
                redirectLocation = "Team/Volunteers";
            }

            _context.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToPage( redirectLocation, new { statusMessage = $"You have successfully deleted {user.VolunteerProfile.FirstName} {user.VolunteerProfile.LastName} volunteer." });
        }

        private async Task UpdateStatus(int statusChangeType)
        {
            var VolunteerProfile = await _context.VolunteerProfiles.FirstOrDefaultAsync(u => u.Id == DetailsModel.Id);

            switch (statusChangeType)
            {
                case 1:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.CriminalRecordCheck = !VolunteerProfile.CriminalRecordCheck;
                    break;
                case 2:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.DrivingAbstract = !VolunteerProfile.DrivingAbstract;
                    break;
                case 3:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.ConfirmationOfProfessionalDesignation = !VolunteerProfile.ConfirmationOfProfessionalDesignation;
                    break;
                case 4:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.ChildWelfareCheck = !VolunteerProfile.ChildWelfareCheck;
                    break;
                case 6:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.FoodSafe = !VolunteerProfile.FoodSafe;
                    break;
                case 8:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.FirstAidCpr = !VolunteerProfile.FirstAidCpr;
                    break;
            }
        }

        private async Task<bool> UpdateUserProfile(AppUser user)
        {
            // get positions for display
            Positions = await _context.Positions.ToListAsync();

            // tag it for change
            _context.Update(user);

            // keep all properties that won't be edited
            DetailsModel.Id = user.VolunteerProfile.Id;
            DetailsModel.FirstName = user.VolunteerProfile.FirstName;
            DetailsModel.LastName = user.VolunteerProfile.LastName;

            // for all properties that can be edited, replace old values with new values via model
            VolunteerProfile newVolunteerProfile = _mapper.Map<VolunteerProfile>(DetailsModel);
            newVolunteerProfile.User = user;
            newVolunteerProfile.UserID = user.Id;
            user.VolunteerProfile = newVolunteerProfile;
            // email is attached to the AppUser entity so it has to be updated individually
            user.Email = DetailsModel.Email;

            // update preferred and assigned positions
            UpdateVolunteerPositions(user.VolunteerProfile);
            await _context.SaveChangesAsync();
            return true;
        }

        private int GetMaxAvailabilityCount()
        {
            int i = 0;
            foreach (List<Availability> availabilities in Availability.Values)
            {
                if (availabilities.Count > i)
                {
                    i = availabilities.Count;
                }
            }
            return i;
        }

        private void UpdateVolunteerPositions(VolunteerProfile volunteer)
        {
            // clear out all current preferences for this volunteer
            List<PositionVolunteer> oldPositions = _context.PositionVolunteers
                .Where(p => p.Volunteer == volunteer).ToList();
            _context.PositionVolunteers.RemoveRange(oldPositions);

            // load new position selection into volunteerprofile
            volunteer.Positions = new List<PositionVolunteer>();

            // iterate through all positions (via model property that contains every selectable position)
            foreach (Position position in Positions)
            {
                // prepare an entity to be added to the volunteer's list of positions
                PositionVolunteer posVol = new PositionVolunteer()
                {
                    Volunteer = volunteer,
                    Position = position,
                };

                // for each position, check to see if it was selected as a preferred position or an assigned position
                var preferred = Request.Form["preferred-" + position.Name];
                var assigned = Request.Form["assigned-" + position.Name];

                bool selectedAsPreferredPosition = preferred.Count > 0 && assigned.Count == 0;
                bool selectedAsAssignedPosition = preferred.Count == 0 && assigned.Count > 0;
                bool selectedAsBoth = preferred.Count > 0 && assigned.Count > 0;

                if (selectedAsPreferredPosition)
                {
                    posVol.Association = PositionVolunteer.AssociationType.Preferred;
                    volunteer.Positions.Add(posVol);
                }
                else if (selectedAsAssignedPosition)
                {
                    posVol.Association = PositionVolunteer.AssociationType.Assigned;
                    volunteer.Positions.Add(posVol);
                }
                else if (selectedAsBoth)
                {
                    posVol.Association = PositionVolunteer.AssociationType.PreferredAndAssigned;
                    volunteer.Positions.Add(posVol);
                }
            }
        }

        private void PrepareModel(int id)
        {
            var volunteerUserProfile = _context.Users
                .Include(p => p.VolunteerProfile)
                .Include(p => p.VolunteerProfile.WorkExperiences)
                .Include(p => p.VolunteerProfile.References)
                .Include(p => p.VolunteerProfile.Availabilities)
                .Include(p => p.VolunteerProfile.Positions).ThenInclude(p => p.Position)
                .FirstOrDefault(u => u.VolunteerProfile.Id == id);

            DetailsModel = _mapper.Map<VolunteerAdminReadEditDto>(volunteerUserProfile.VolunteerProfile);
            DetailsModel.Email = volunteerUserProfile.Email;

            Positions = _context.Positions.Where(p => p.Name != "All" && !p.Deleted).ToList();

            AvailabilityHandler availability = new AvailabilityHandler();
            OldAvailability = availability.GetSortedAvailability(volunteerUserProfile.VolunteerProfile.Availabilities);

            CurrentPage = $"Volunteer Details for {volunteerUserProfile.VolunteerProfile.FirstName} {volunteerUserProfile.VolunteerProfile.LastName}";
        }
    }
}