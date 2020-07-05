using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Volunteer.Pages
{
    [BindProperties]
    public class VolunteerCalendarModel : VolunteerPageModel
    {
        public List<Shift> UserShifts { get; set; }
        public List<Shift> OpenShifts { get; set; }
        public List<Position> Positions { get; set; }
        public bool AddedShift { get; set; } = false;
        public string StatusMessage { get; set; }

        public VolunteerCalendarModel(FoodBankContext context, UserManager<AppUser> userManager) : base(userManager, context)
        {

        }

        public async Task OnGetAsync(string statusMessage = null)
        {
            StatusMessage = statusMessage;
            await PrepareModelAndGetCurrentVolunteer();
        }

        public async Task<IActionResult> OnPostWorkShift()
        {
            var volunteer = await PrepareModelAndGetCurrentVolunteer();

            int selectedShiftId = Convert.ToInt32(Request.Form["open-shift-id"]);
            Shift selectedShift = _context.Shifts.FirstOrDefault(s => s.Id == selectedShiftId);

            await AssignShiftToVolunteer(selectedShift, volunteer);

            return RedirectToPage(new { statusMessage = "You have signed up for the selected shift!"});
        }

        public async Task<IActionResult> OnPostRequestChange()
        {
            int originalShiftId = Convert.ToInt32(Request.Form["assigned-shift-id"]);
            string shiftDate = null;

            Shift selectedShift = await _context.Shifts.FirstOrDefaultAsync(s => s.Id == originalShiftId);

            // if the original shift is part of a recurring set, 
            // the RequestChange page needs to know the date of the selected shift
            if (selectedShift is RecurringShift recurringShift)
            {
                shiftDate = Convert.ToDateTime(Request.Form["assigned-shift-date"]).ToString("yyyy-MM-dd");
            }

            return RedirectToPage("RequestChange", new { oldShiftId = originalShiftId, originalShiftDate = shiftDate});
        }
        private async Task<VolunteerProfile> PrepareModelAndGetCurrentVolunteer()
        {
            var user = await _userManager.GetUserAsync(User);
            await _context.Entry(user).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(user.VolunteerProfile).Collection(p => p.Shifts).LoadAsync();
            UserShifts = _context.Shifts.Where(s => s.Hidden == false && s.Volunteer.Id == user.VolunteerProfile.Id).ToList();
            OpenShifts = _context.Shifts.Where(s => s.Hidden == false && s.Volunteer == null).ToList();
            Positions = _context.Positions.ToList();
            foreach (Shift shift in UserShifts)
            {
                await _context.Entry(shift).Reference(p => p.PositionWorked).LoadAsync();
            }
            foreach (Shift shift in OpenShifts)
            {
                await _context.Entry(shift).Reference(p => p.PositionWorked).LoadAsync();
            }
            LoggedInUser = user.VolunteerProfile.FirstName + " " + user.VolunteerProfile.LastName;

            return user.VolunteerProfile;
        }

        // this method requires the entire AppUser entity because it contains the user's email, and a 
        private async Task AssignShiftToVolunteer(Shift selectedShift, VolunteerProfile volunteer)
        {
            await _context.Entry(selectedShift).Reference(p => p.PositionWorked).LoadAsync();
            _context.Update(selectedShift);

            if (selectedShift is RecurringShift recShift)
            {
                await _context.Entry(recShift).Collection(p => p.ExcludedShifts).LoadAsync();
                Shift excludedShift = new Shift()
                {
                    StartDate = Convert.ToDateTime(Request.Form["open-shift-date"]),
                    StartTime = selectedShift.StartTime,
                    EndTime = selectedShift.EndTime,
                    Hidden = false,
                    PositionWorked = selectedShift.PositionWorked,
                    Volunteer = volunteer
                };
                excludedShift.CreateDescription();
                await _context.AddAsync(excludedShift);
                recShift.ExcludedShifts.Add(excludedShift);

                bool allShiftsInRecurringSetAreExcluded = recShift.ConstituentShifts.Count == recShift.ExcludedShifts.Count;

                if (allShiftsInRecurringSetAreExcluded)
                {
                    _context.Remove(recShift);
                }

                recShift.UpdateRecurrenceRule();

                // schedule email notification for shift
                //ReminderScheduler.ScheduleReminder(.Email, CurrentUser.VolunteerProfile, excludedShift, _context);
            }
            else
            {
                selectedShift.Volunteer = volunteer;
                selectedShift.CreateDescription();
                // schedule email notification for shift
            //    ReminderScheduler.ScheduleReminder(CurrentUser.Email, CurrentUser.VolunteerProfile, selectedShift, _context);
            }
            await _context.SaveChangesAsync();
        }
    }
}