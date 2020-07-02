using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using MHFoodBank.Web.Dtos;
using AutoMapper;

namespace MHFoodBank.Web.Areas.Admin.Pages.Shared
{
    [Authorize(Roles = "Staff, Admin")]
    public class VolunteerDetailsModel : AdminPageModel
    {
        [BindProperty]
        public VolunteerAdminReadEditDto DetailsModel { get; set; }
        [BindProperty]
        public bool test { get; set; }
        [BindProperty] 
        public Dictionary<string, List<Availability>> Availability { get; set; } = new Dictionary<string, List<Availability>>();
        [BindProperty]
        public int MaxAvailabilityCount { get { return GetMaxAvailabilityCount(); } }

        [BindProperty] 
        public List<Position> Positions { get; set; }
        [BindProperty] public string StatusMessage { get; set; }

        private readonly IMapper _mapper;

        public VolunteerDetailsModel(FoodBankContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public void OnGet(int id, string statusMessage = null)
        {
            StatusMessage = statusMessage;
            PrepareModel(id);
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(int check, int id)
        {
            PrepareModel(id);
            await UpdateStatus(check);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = id});
        }

        // id == VolunteerProfile id
        public async Task<IActionResult> OnPostSaveChangesAsync(int id)
        {
            await UpdateUserProfile(id);
            AppUser user = await _context.Users
                .Include(p => p.VolunteerProfile)
                .FirstOrDefaultAsync(p => p.VolunteerProfile.Id == id);
            StatusMessage = $"You successfully saved the changes to {user.VolunteerProfile.FirstName} {user.VolunteerProfile.LastName}'s profile.";

            return RedirectToPage(new { id = user.VolunteerProfile.Id, statusMessage = StatusMessage});
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
                case 5:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.OfficiallyApproved = !VolunteerProfile.OfficiallyApproved;
                    break;
                case 6:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.FoodSafe = !VolunteerProfile.FoodSafe;
                    break;
                case 7:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.Cpr = !VolunteerProfile.Cpr;
                    break;
                case 8:
                    _context.Entry(VolunteerProfile).State = EntityState.Modified;
                    VolunteerProfile.FirstAid = !VolunteerProfile.FirstAid;
                    break;
            }
        }

        private async Task UpdateUserProfile(int userId)
        {
            // get positions for display
            Positions = await _context.Positions.ToListAsync();

            // retrieve the user to be updated and load volunteer profile
            AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == userId);
            await _context.Entry(user).Reference(p => p.VolunteerProfile).LoadAsync();

            // tag it for change
            _context.Update(user);
            
            // load volunteer profile and all navigation properties
            VolunteerProfile volunteerProfile = user.VolunteerProfile;
            await _context.Entry(volunteerProfile).Collection(p => p.Availabilities).LoadAsync();
            await _context.Entry(volunteerProfile).Collection(p => p.References).LoadAsync();
            await _context.Entry(volunteerProfile).Collection(p => p.WorkExperiences).LoadAsync();

            // keep all properties that won't be edited
            DetailsModel.Id = volunteerProfile.Id;
            DetailsModel.FirstName = volunteerProfile.FirstName;
            DetailsModel.LastName = volunteerProfile.LastName;

            // for all properties that can be edited, replace old values with new values via model
            user.Email = DetailsModel.Email;
            VolunteerProfile newVolunteerProfile = _mapper.Map<VolunteerProfile>(DetailsModel);
            user.VolunteerProfile = newVolunteerProfile;

            // update preferred and assigned positions
            UpdateVolunteerPositions(user.VolunteerProfile);
            _context.Entry(volunteerProfile).State = EntityState.Detached;
            await _context.SaveChangesAsync();
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
            volunteer.Positions = new List<PositionVolunteer>();
            foreach (Position position in Positions)
            {
                var preferred = Request.Form["preferred-" + position.Name];
                var assigned = Request.Form["assigned-" + position.Name];

                PositionVolunteer posVol = new PositionVolunteer()
                {
                    Volunteer = volunteer,
                    Position = position,
                };

                // this means it was preferred, StringValues are weird
                if (preferred.Count > 0 && assigned.Count == 0)
                {
                    posVol.Association = PositionVolunteer.AssociationType.Preferred;
                    volunteer.Positions.Add(posVol);
                }
                // this means it was assigned
                else if (preferred.Count == 0 && assigned.Count > 0)
                {
                    posVol.Association = PositionVolunteer.AssociationType.Assigned;
                    volunteer.Positions.Add(posVol);
                }
                // this means it was both
                else if (preferred.Count > 0 && assigned.Count > 0)
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

            Positions = _context.Positions.Where(p => p.Name != "All").ToList();
            
            GetSortedAvailability(volunteerUserProfile.VolunteerProfile.Availabilities);
        }

        private void GetSortedAvailability(IList<Availability> availabilities)
        {
            string[] daysInWeek = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };

            foreach (string day in daysInWeek)
            {
                List<Availability> currentDayAvailbilities = availabilities.Where(a => a.AvailableDay == day).OrderBy(a => a.StartTime).ToList();
                Availability.Add(day, currentDayAvailbilities);
            }
        }

        public class VolunteerDetails
        {
            public string Email { get; set; }
            
        }
    }
}