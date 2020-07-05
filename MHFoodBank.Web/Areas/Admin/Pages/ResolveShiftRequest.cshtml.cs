using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin")]
    public class ResolveShiftRequestModel : AdminPageModel
    {
        public enum RequestResolvePageViewType
        {
            NewSwitch,
            NewRemoval,
            ArchivedSwitch,
            ArchivedRemoval
        }

        // when ShiftRequest.NewShift is null it means its a request to remove shift entirely
        public ShiftRequestAlert ShiftRequest { get; set; }
        public List<Shift> OpenShifts { get; set; }
        public List<Shift> AssignedShifts { get; set; }
        public RequestResolvePageViewType ViewType { get; set; }

        public ResolveShiftRequestModel(FoodBankContext context, string currentPage = "Shift Request") : base(context, currentPage)
        {

        }

        public async Task OnGet(int alertId, string requestType)
        {
            ShiftRequest = await LoadAlertForDisplay(alertId);
            if (ShiftRequest.Status == ShiftRequestAlert.RequestStatus.Pending)
            {
                OpenShifts = await LoadOpenShifts();
                AssignedShifts = await LoadAssignedShifts();
            }
            // if shift request is for a switch and the requested shift is part of a recurring set, 
            // this method will ensure that only one instance of that shift will be displayed
            ResolveRecurringShiftDisplay();
        }

        public async Task<IActionResult> OnPostAccept(int alertId)
        {
            ShiftRequest = await LoadAlertForResolution(alertId);
            OpenShifts = await LoadOpenShifts();
            AssignedShifts = await LoadAssignedShifts();

            _context.Update(ShiftRequest);
            bool isRemovalRequest = ShiftRequest.NewShift == null;

            if (isRemovalRequest != true)
            {
                await ResolveSwitchRequest();
            }
            else
            {
                // the calendars only display shifts whose archived property == false so this alert won't be displayed
                // this is so the shift will be preserved and the alert can still be viewed in the volunteer's and admin's request archive
                // a better system may be worth creating to solve this problem
                ShiftRequest.OldShift.Hidden = true;

                Shift removedShift = ShiftRequest.OldShift;

                await ReminderScheduler.CancelReminder(removedShift, _context);
            }

            // this is to control the displayed status of the alert in the volunteer inbox
            ShiftRequest.Status = ShiftRequestAlert.RequestStatus.Accepted;
            
            await _context.SaveChangesAsync();

            return RedirectToPage("Alerts", new { statusMessage = "You accepted the shift change request" });
        }

        public async Task<IActionResult> OnPostDecline(int alertId)
        {
            ShiftRequest = await LoadAlertForResolution(alertId);
            _context.Update(ShiftRequest);
            // no changes to the requestalert's contents are made upon decline
            ShiftRequest.Status = ShiftRequestAlert.RequestStatus.Declined;
            await _context.SaveChangesAsync();
            return RedirectToPage("Alerts", new { statusMessage = "You declined the shift change request" });
        }

        private async Task ResolveSwitchRequest()
        {
            // put all information that needs to be switched into external variables for readability
            VolunteerProfile oldShiftVolunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == ShiftRequest.OldShift.Volunteer.Id);
            Position oldShiftPosition = await _context.Positions.FirstOrDefaultAsync(v => v.Id == ShiftRequest.OldShift.PositionWorked.Id);
            string oldShiftTitleDescription = ShiftRequest.OldShift.Description;
            VolunteerProfile newShiftVolunteer = null;

            if (ShiftRequest.NewShift.Volunteer != null)
            {
                newShiftVolunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == ShiftRequest.NewShift.Volunteer.Id);
            }

            Position newShiftPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == ShiftRequest.NewShift.PositionWorked.Id);
            string newShiftTitleDescription = ShiftRequest.NewShift.Description;

            // same with all the info that will stay the same
            DateTime oldStartDate = ShiftRequest.OldShift.StartDate;
            TimeSpan oldStartTime = ShiftRequest.OldShift.StartTime;
            TimeSpan oldEndTime = ShiftRequest.OldShift.EndTime;
            RecurringShift oldRecurringShift = ShiftRequest.OldShift.ParentRecurringShift;

            DateTime newStartDate = ShiftRequest.NewShift.StartDate;
            TimeSpan newStartTime = ShiftRequest.NewShift.StartTime;
            TimeSpan newEndTime = ShiftRequest.NewShift.EndTime;
            RecurringShift newRecurringShift = ShiftRequest.NewShift.ParentRecurringShift;

            // create two new shifts
            Shift updatedOldShift = new Shift();
            Shift updatedNewShift = new Shift();

            // switch volunteer, description of shifts, position worked
            updatedOldShift.Volunteer = newShiftVolunteer;
            updatedNewShift.Volunteer = oldShiftVolunteer;

            updatedOldShift.Description = newShiftTitleDescription;
            updatedNewShift.Description = oldShiftTitleDescription;

            updatedOldShift.PositionWorked = newShiftPosition;
            updatedNewShift.PositionWorked = oldShiftPosition;

            // fill in the rest of the information
            updatedOldShift.StartDate = oldStartDate;
            updatedOldShift.StartTime = oldStartTime;
            updatedOldShift.EndTime = oldEndTime;
            updatedOldShift.ParentRecurringShift = oldRecurringShift;

            updatedNewShift.StartDate = newStartDate;
            updatedNewShift.StartTime = newStartTime;
            updatedNewShift.EndTime = newEndTime;
            updatedNewShift.ParentRecurringShift = newRecurringShift;

            if (oldRecurringShift != null)
            {
                // load recurring set's excluded shifts so the newshift can be added to it
                await _context.Entry(updatedOldShift.ParentRecurringShift).Collection(p => p.ExcludedShifts).LoadAsync();

                // exclude the new shift from the recurring set and remove the old version 
                updatedOldShift.ParentRecurringShift.ExcludedShifts.Add(updatedOldShift);
                updatedOldShift.ParentRecurringShift.ExcludedShifts.Remove(ShiftRequest.OldShift);
                updatedOldShift.ParentRecurringShift.UpdateRecurrenceRule();
            }

            if (newRecurringShift != null)
            {
                // load recurring set's excluded shifts so the newshift can be added to it
                await _context.Entry(updatedNewShift.ParentRecurringShift).Collection(p => p.ExcludedShifts).LoadAsync();

                // exclude the new shift from the recurring set
                updatedNewShift.ParentRecurringShift.ExcludedShifts.Add(updatedNewShift);
                updatedNewShift.ParentRecurringShift.ExcludedShifts.Remove(ShiftRequest.NewShift);
                updatedNewShift.ParentRecurringShift.UpdateRecurrenceRule();
            }

            await _context.AddAsync(updatedOldShift);
            await _context.AddAsync(updatedNewShift);

            // hide old versions of shifts shifts
            ShiftRequest.OldShift.Hidden = true;
            ShiftRequest.NewShift.Hidden = true;

            ShiftRequest.AddressedBy = User.Identity.Name;

            await _context.SaveChangesAsync();

            // schedule reminders, this must be done while the old shifts still have parentrecurringshifts
            ScheduleRemindersAfterSwitch(updatedOldShift, updatedNewShift);

        }
        
        private void ScheduleRemindersAfterSwitch(Shift OldShift, Shift NewShift)
        {
            // cancel old reminders; this must be done first, if reminders are scheduled first, 
            // savechanges will be called and parentrecurring shifts will be for ShiftRequest.OldShift and ShiftRequest.NewShift will be null
            if (ShiftRequest.OldShift.Volunteer != null)
            {
                if (ShiftRequest.OldShift.ParentRecurringShift != null)
                {
                    ReminderScheduler.CancelReminder(ShiftRequest.OldShift.ParentRecurringShift, _context, ShiftRequest.OldShift.StartDate);
                }
                else
                {
                    ReminderScheduler.CancelReminder(ShiftRequest.OldShift, _context);
                }
            }

            if (ShiftRequest.NewShift.Volunteer != null)
            {
                if (ShiftRequest.NewShift.ParentRecurringShift != null)
                {
                    ReminderScheduler.CancelReminder(ShiftRequest.NewShift.ParentRecurringShift, _context, ShiftRequest.NewShift.StartDate);
                }
                else
                {
                    ReminderScheduler.CancelReminder(ShiftRequest.NewShift, _context);
                }
            }

            // schedule new reminders
            if (OldShift.Volunteer != null)
            {
                if (OldShift.ParentRecurringShift != null)
                {
                    ReminderScheduler.ScheduleReminder(OldShift.Volunteer.User.Email, OldShift.Volunteer, OldShift, _context, OldShift.StartDate);
                }
                else
                {
                    ReminderScheduler.ScheduleReminder(OldShift.Volunteer.User.Email, OldShift.Volunteer, OldShift, _context);
                }
            }

            if (NewShift.Volunteer != null)
            {
                if (NewShift.ParentRecurringShift != null)
                {
                    ReminderScheduler.ScheduleReminder(NewShift.Volunteer.User.Email, NewShift.Volunteer, NewShift, _context, NewShift.StartDate);
                }
                else
                {
                    ReminderScheduler.ScheduleReminder(NewShift.Volunteer.User.Email, NewShift.Volunteer, NewShift, _context);
                }
            }
        }

        private async Task<List<Shift>> LoadOpenShifts()
        {
            return await _context.Shifts
                .Include(p => p.Volunteer)
                .Include(p => p.PositionWorked)
                .Where(
                    s => s.Hidden == false &&
                            s != ShiftRequest.NewShift &&
                            s != ShiftRequest.OldShift &&
                            s.Volunteer == null).ToListAsync();
        }

        private async Task<List<Shift>> LoadAssignedShifts()
        {
                return await _context.Shifts
                    .Include(p => p.Volunteer)
                    .Include(p => p.PositionWorked)
                    .Where(
                        s => s.Hidden == false &&
                             s != ShiftRequest.NewShift &&
                             s != ShiftRequest.OldShift &&
                             s.Volunteer != null).ToListAsync();
        }

        private async Task<ShiftRequestAlert> LoadAlertForResolution(int alertId)
        {
            ShiftRequestAlert selectedAlert = await _context.ShiftAlerts.FirstOrDefaultAsync(p => p.Id == alertId);

            // load oldshift properties
            await _context.Entry(selectedAlert).Reference(p => p.OldShift).LoadAsync();
            await _context.Entry(selectedAlert.OldShift).Reference(p => p.PositionWorked).LoadAsync();
            await _context.Entry(selectedAlert.OldShift).Reference(p => p.Volunteer).LoadAsync();
            await _context.Entry(selectedAlert.OldShift.Volunteer).Reference(p => p.User).LoadAsync();
            await _context.Entry(selectedAlert.OldShift).Reference(p => p.ParentRecurringShift).LoadAsync();

            // load newshift properties if its not null
            if (ShiftRequest.NewShift != null)
            {
                await _context.Entry(selectedAlert).Reference(p => p.NewShift).LoadAsync();
                await _context.Entry(selectedAlert.NewShift).Reference(p => p.PositionWorked).LoadAsync();
                await _context.Entry(selectedAlert.NewShift).Reference(p => p.Volunteer).LoadAsync();
                await _context.Entry(selectedAlert.NewShift).Reference(p => p.ParentRecurringShift).LoadAsync();

                if (selectedAlert.NewShift.Volunteer != null)
                {
                    await _context.Entry(selectedAlert.NewShift.Volunteer).Reference(p => p.User).LoadAsync();
                }
            }

            return selectedAlert;
        }

        private async Task<ShiftRequestAlert> LoadAlertForDisplay(int alertId)
        {
            ShiftRequestAlert shiftRequest = await _context.ShiftAlerts.FirstOrDefaultAsync(p => p.Id == alertId);
            await _context.Entry(shiftRequest).Reference(p => p.Volunteer).LoadAsync();
            await _context.Entry(shiftRequest).Reference(p => p.OldShift).LoadAsync();
            await _context.Entry(shiftRequest.OldShift).Reference(p => p.PositionWorked).LoadAsync();
            await _context.Entry(shiftRequest.OldShift).Reference(p => p.Volunteer).LoadAsync();
            await _context.Entry(shiftRequest).Reference(p => p.NewShift).LoadAsync();

            if (shiftRequest.NewShift != null)
            {
                await _context.Entry(shiftRequest.NewShift).Reference(p => p.PositionWorked).LoadAsync();
                await _context.Entry(shiftRequest.NewShift).Reference(p => p.Volunteer).LoadAsync();
            }

            await _context.Entry(shiftRequest).Reference(p => p.Volunteer).LoadAsync();

            return shiftRequest;
        }

        private void ResolveRecurringShiftDisplay()
        {
            bool isSwitchRequest = ShiftRequest.OldShift.ParentRecurringShift != null;

            _context.Entry(ShiftRequest.OldShift).Reference(p => p.ParentRecurringShift).LoadAsync();
            bool originalShiftIsPartOfRecurringSet = ShiftRequest.OldShift.ParentRecurringShift != null;

            bool requestedShiftIsPartOfRecurringSet = false;
            if (isSwitchRequest)
            {
                _context.Entry(ShiftRequest.NewShift).Reference(p => p.ParentRecurringShift).LoadAsync();
                requestedShiftIsPartOfRecurringSet = ShiftRequest.NewShift.ParentRecurringShift != null;
            }


            if (originalShiftIsPartOfRecurringSet)
            {
                ShiftRequest.NewShift.Hidden = false;

                DateTime selectedShiftDate = ShiftRequest.OldShift.StartDate;
                TimeSpan selectedShiftTime = ShiftRequest.OldShift.StartTime;

                DateTime combinedDateTime = new DateTime(
                    selectedShiftDate.Year,
                    selectedShiftDate.Month,
                    selectedShiftDate.Day,
                    selectedShiftTime.Hours,
                    selectedShiftTime.Minutes,
                    selectedShiftTime.Seconds);

                string excludedDateString = $"\\nEXDATE:{combinedDateTime.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}Z";

                // check both lists of shifts for the recurring shift that is the selected shift's parent
                bool recurringShiftIsOpen = OpenShifts.Any(s => s == ShiftRequest.OldShift.ParentRecurringShift);

                // once the recurring shift is found, exclude the selected shift from its set
                if (recurringShiftIsOpen)
                {
                    ((RecurringShift)OpenShifts.FirstOrDefault(s => s == ShiftRequest.OldShift.ParentRecurringShift)).RecurrenceRule += excludedDateString;
                }
                else
                {
                    ((RecurringShift)AssignedShifts.FirstOrDefault(s => s == ShiftRequest.OldShift.ParentRecurringShift)).RecurrenceRule += excludedDateString;
                }

                _context.Entry(ShiftRequest.OldShift.ParentRecurringShift).State = EntityState.Detached;
            }

            if (isSwitchRequest)
            {
                if (requestedShiftIsPartOfRecurringSet)
                {
                    ShiftRequest.NewShift.Hidden = false;

                    DateTime selectedShiftDate = ShiftRequest.NewShift.StartDate;
                    TimeSpan selectedShiftTime = ShiftRequest.NewShift.StartTime;

                    DateTime combinedDateTime = new DateTime(
                        selectedShiftDate.Year,
                        selectedShiftDate.Month,
                        selectedShiftDate.Day,
                        selectedShiftTime.Hours,
                        selectedShiftTime.Minutes,
                        selectedShiftTime.Seconds);

                    string excludedDateString = $"\\nEXDATE:{combinedDateTime.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}Z";

                    // check both lists of shifts for the recurring shift that is the selected shift's parent
                    bool recurringShiftIsOpen = OpenShifts.Any(s => s == ShiftRequest.NewShift.ParentRecurringShift);

                    // once the recurring shift is found, exclude the selected shift from its set
                    if (recurringShiftIsOpen)
                    {
                        ((RecurringShift)OpenShifts.FirstOrDefault(s => s == ShiftRequest.NewShift.ParentRecurringShift)).RecurrenceRule += excludedDateString;
                    }
                    else
                    {
                        ((RecurringShift)AssignedShifts.FirstOrDefault(s => s == ShiftRequest.NewShift.ParentRecurringShift)).RecurrenceRule += excludedDateString;
                    }

                    _context.Entry(ShiftRequest.NewShift.ParentRecurringShift).State = EntityState.Detached;
                }
            }
        }
    }
}