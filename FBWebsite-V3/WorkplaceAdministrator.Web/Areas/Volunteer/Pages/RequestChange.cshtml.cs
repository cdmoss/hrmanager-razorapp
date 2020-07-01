using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WorkplaceAdministrator.Web.Areas.Volunteer.Pages.Shared;
using WorkplaceAdministrator.Web.Data;
using WorkplaceAdministrator.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WorkplaceAdministrator.Web.Areas.Volunteer.Pages
{
    [BindProperties]
    public class RequestChangeModel : VolunteerPageModel
    {
        public List<Shift> TakenShifts { get; set; }
        public List<Shift> OpenShifts { get; set; }
        public Shift OldShift { get; set; }
        public DateTime IndividualDate { get; set; }
        public Shift NewShift { get; set; }
        public string Reason { get; set; }
        public List<Position> Positions { get; set; }
        public int OldShiftId { get; set; }

        public RequestChangeModel(FoodBankContext context, UserManager<AppUser> userManager) : base(userManager, context)
        {

        }

        public async Task OnGet(int oldShiftId, string oldShiftDate = null)
        {
            VolunteerProfile currentVolunteer = await PrepareModel(oldShiftId, oldShiftDate);
        }

        public async Task<IActionResult> OnPostChange()
        {
            // this can be optimized, fillmodel doesnt have to retrieve all properties for tis method
            VolunteerProfile currentVolunteer = await PrepareModel(OldShiftId);

            // get selected shift
            Shift shiftToBeSwitched = await _context.Shifts.FirstOrDefaultAsync(s => s.Id == Convert.ToInt32(Request.Form["ShiftID"]));

            // create skeleton of the alert, leave shifts null
            ShiftRequestAlert requestAlert = new ShiftRequestAlert()
            {
                Date = DateTime.Now,
                Reason = Reason,
                Volunteer = currentVolunteer,
                Status = ShiftRequestAlert.RequestStatus.Pending
            };

            if (OldShift is RecurringShift oldRecurringShift)
            {
                // if selected shift is recurring shift, create a new shift with a date that matches the selected shift, hide it so it won't
                // display until the request is accepted
                Shift selectedShiftFromRecurringSet = new Shift()
                {
                    StartDate = IndividualDate,
                    Hidden = true,
                    StartTime = oldRecurringShift.StartTime,
                    EndTime = oldRecurringShift.EndTime,
                    ParentRecurringShift = oldRecurringShift,
                    Volunteer = oldRecurringShift.Volunteer,
                    PositionWorked = oldRecurringShift.PositionWorked
                };
                selectedShiftFromRecurringSet.CreateDescription();

                // add created shift to the alert
                requestAlert.OldShift = selectedShiftFromRecurringSet;
            }
            else
            {
                // simply add the selected shift to the alert
                requestAlert.OldShift = OldShift;
            }

            if (shiftToBeSwitched is RecurringShift newRecurringShift)
            {
                // if selected shift is recurring shift, create a new shift with a date that matches the selected shift, hide it so it won't
                // display until the request is accepted
                Shift selectedShiftFromRecurringSet = new Shift()
                {
                    StartDate = Convert.ToDateTime(Request.Form["ShiftDate"]),
                    Hidden = true,
                    StartTime = newRecurringShift.StartTime,
                    EndTime = newRecurringShift.EndTime,
                    ParentRecurringShift = newRecurringShift,
                    Volunteer = newRecurringShift.Volunteer,
                    PositionWorked = newRecurringShift.PositionWorked
                };
                selectedShiftFromRecurringSet.CreateDescription();

                // add created shift to the alert
                requestAlert.NewShift = selectedShiftFromRecurringSet;
            }
            else
            {
                // simply add the selected shift to the alert
                requestAlert.NewShift = shiftToBeSwitched;
            }

            _context.ShiftAlerts.Add(requestAlert);
            await _context.SaveChangesAsync();

            return RedirectToPage("/VolunteerCalendar", new { statusMessage = "You successfully requested to change your shift." });
        }

        public async Task<IActionResult> OnPostRemove()
        {
            // this can be optimized, fillmodel doesnt have to retrieve all properties for tis method
            VolunteerProfile currentVolunteer = await PrepareModel(OldShiftId);

            ShiftRequestAlert requestAlert = new ShiftRequestAlert()
            {
                Date = DateTime.Now,
                OldShift = OldShift,
                Reason = Reason,
                Volunteer = currentVolunteer,
                Status = ShiftRequestAlert.RequestStatus.Pending
            };
            _context.ShiftAlerts.Add(requestAlert);
            await _context.SaveChangesAsync();

            return RedirectToPage("/VolunteerCalendar", new { statusMessage = "You successfully requested to remove your shift." });
        }

        private async Task<VolunteerProfile> PrepareModel(int oldShiftId, string oldShiftDate = null)
        {
            AppUser currentUser = await _userManager.GetUserAsync(User);
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            //TODO: rename to shifts with volunteers
            List<Shift> shifts = _context.Shifts.Include(p => p.Volunteer).Where(s => s.Hidden == false && s.Volunteer != null).ToList();
            TakenShifts = shifts.Where(s => s.Hidden == false && s.Volunteer.Id != currentUser.VolunteerProfile.Id).ToList();
            OpenShifts = _context.Shifts.Include(p => p.Volunteer).Where(s => s.Hidden == false && s.Volunteer == null).ToList();
            await _context.Entry(currentUser.VolunteerProfile).Collection(p => p.Shifts).LoadAsync();

            if (oldShiftDate != null)
            {
                OldShift = await _context.RecurringShifts.FirstOrDefaultAsync(s => s.Id == oldShiftId);
                IndividualDate = Convert.ToDateTime(oldShiftDate);
            }
            OldShift = await _context.Shifts.FirstOrDefaultAsync(s => s.Id == oldShiftId);


            await _context.Entry(OldShift).Reference(p => p.PositionWorked).LoadAsync();


            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
            Positions = await _context.Positions.ToListAsync();

            return currentUser.VolunteerProfile;
        }
    }
}