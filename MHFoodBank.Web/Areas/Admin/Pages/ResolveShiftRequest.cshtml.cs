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
        private readonly IReminderManager _reminderManager;
        public enum RequestResolvePageViewType
        {
            NewSwitch,
            NewRemoval,
            ArchivedSwitch,
            ArchivedRemoval
        }

        // when ShiftRequest.RequestedShift is null it means its a request to remove shift entirely
        public ShiftRequestAlert ShiftRequest { get; set; }
        public List<Shift> OpenShifts { get; set; }
        public List<Shift> AssignedShifts { get; set; }
        public RequestResolvePageViewType ViewType { get; set; }

        public ResolveShiftRequestModel(FoodBankContext context, IReminderManager reminderManager, string currentPage = "Shift Request") : base(context, currentPage)
        {
            _reminderManager = reminderManager;
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
            bool isRemovalRequest = ShiftRequest.RequestedShift == null;

            if (isRemovalRequest != true)
            {
                await ResolveSwitchRequest();
            }
            else
            {
                // the calendars only display shifts whose archived property == false so this alert won't be displayed
                // this is so the shift will be preserved and the alert can still be viewed in the volunteer's and admin's request archive
                // a better system may be worth creating to solve this problem
                ShiftRequest.OriginalShift.Hidden = true;

                Shift removedShift = ShiftRequest.OriginalShift;

                _reminderManager.CancelReminder(removedShift);
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
            VolunteerProfile oldShiftVolunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == ShiftRequest.OriginalShift.Volunteer.Id);
            Position oldShiftPosition = await _context.Positions.FirstOrDefaultAsync(v => v.Id == ShiftRequest.OriginalShift.PositionWorked.Id);
            string oldShiftTitleDescription = ShiftRequest.OriginalShift.Description;
            VolunteerProfile newShiftVolunteer = null;

            if (ShiftRequest.RequestedShift.Volunteer != null)
            {
                newShiftVolunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == ShiftRequest.RequestedShift.Volunteer.Id);
            }

            Position newShiftPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == ShiftRequest.RequestedShift.PositionWorked.Id);
            string newShiftTitleDescription = ShiftRequest.RequestedShift.Description;

            // same with all the info that will stay the same
            DateTime oldStartDate = ShiftRequest.OriginalShift.StartDate;
            TimeSpan oldStartTime = ShiftRequest.OriginalShift.StartTime;
            TimeSpan oldEndTime = ShiftRequest.OriginalShift.EndTime;
            RecurringShift oldRecurringShift = ShiftRequest.OriginalShift.ParentRecurringShift;

            DateTime newStartDate = ShiftRequest.RequestedShift.StartDate;
            TimeSpan newStartTime = ShiftRequest.RequestedShift.StartTime;
            TimeSpan newEndTime = ShiftRequest.RequestedShift.EndTime;
            RecurringShift newRecurringShift = ShiftRequest.RequestedShift.ParentRecurringShift;

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
                // load recurring set's excluded shifts so the RequestedShift can be added to it
                await _context.Entry(updatedOldShift.ParentRecurringShift).Collection(p => p.ExcludedShifts).LoadAsync();

                // exclude the new shift from the recurring set and remove the old version 
                updatedOldShift.ParentRecurringShift.ExcludedShifts.Add(updatedOldShift);
                updatedOldShift.ParentRecurringShift.ExcludedShifts.Remove(ShiftRequest.OriginalShift);
                updatedOldShift.ParentRecurringShift.UpdateRecurrenceRule();
            }

            if (newRecurringShift != null)
            {
                // load recurring set's excluded shifts so the RequestedShift can be added to it
                await _context.Entry(updatedNewShift.ParentRecurringShift).Collection(p => p.ExcludedShifts).LoadAsync();

                // exclude the new shift from the recurring set
                updatedNewShift.ParentRecurringShift.ExcludedShifts.Add(updatedNewShift);
                updatedNewShift.ParentRecurringShift.ExcludedShifts.Remove(ShiftRequest.RequestedShift);
                updatedNewShift.ParentRecurringShift.UpdateRecurrenceRule();
            }

            await _context.AddAsync(updatedOldShift);
            await _context.AddAsync(updatedNewShift);

            // hide old versions of shifts shifts
            ShiftRequest.OriginalShift.Hidden = true;
            ShiftRequest.RequestedShift.Hidden = true;

            ShiftRequest.AddressedBy = User.Identity.Name;

            await _context.SaveChangesAsync();

            // schedule reminders, this must be done while the old shifts still have parentrecurringshifts
            ScheduleRemindersAfterSwitch(updatedOldShift, updatedNewShift);

        }
        
        private void ScheduleRemindersAfterSwitch(Shift OriginalShift, Shift RequestedShift)
        {
            // cancel old reminders; this must be done first, if reminders are scheduled first, 
            // savechanges will be called and parentrecurring shifts will be for ShiftRequest.OriginalShift and ShiftRequest.RequestedShift will be null
            if (ShiftRequest.OriginalShift.Volunteer != null)
            {
                if (ShiftRequest.OriginalShift.ParentRecurringShift != null)
                {
                    _reminderManager.CancelReminder(ShiftRequest.OriginalShift.ParentRecurringShift, ShiftRequest.OriginalShift.StartDate);
                }
                else
                {
                    _reminderManager.CancelReminder(ShiftRequest.OriginalShift);
                }
            }

            if (ShiftRequest.RequestedShift.Volunteer != null)
            {
                if (ShiftRequest.RequestedShift.ParentRecurringShift != null)
                {
                    _reminderManager.CancelReminder(ShiftRequest.RequestedShift.ParentRecurringShift, ShiftRequest.RequestedShift.StartDate);
                }
                else
                {
                    _reminderManager.CancelReminder(ShiftRequest.RequestedShift);
                }
            }

            // schedule new reminders
            if (OriginalShift.Volunteer != null)
            {
                if (OriginalShift.ParentRecurringShift != null)
                {
                    _reminderManager.ScheduleReminder(OriginalShift.Volunteer.User.Email, OriginalShift.Volunteer, OriginalShift, OriginalShift.StartDate);
                }
                else
                {
                    _reminderManager.ScheduleReminder(OriginalShift.Volunteer.User.Email, OriginalShift.Volunteer, OriginalShift);
                }
            }

            if (RequestedShift.Volunteer != null)
            {
                if (RequestedShift.ParentRecurringShift != null)
                {
                    _reminderManager.ScheduleReminder(RequestedShift.Volunteer.User.Email, RequestedShift.Volunteer, RequestedShift, RequestedShift.StartDate);
                }
                else
                {
                    _reminderManager.ScheduleReminder(RequestedShift.Volunteer.User.Email, RequestedShift.Volunteer, RequestedShift);
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
                            s != ShiftRequest.RequestedShift &&
                            s != ShiftRequest.OriginalShift &&
                            s.Volunteer == null).ToListAsync();
        }

        private async Task<List<Shift>> LoadAssignedShifts()
        {
                return await _context.Shifts
                    .Include(p => p.Volunteer)
                    .Include(p => p.PositionWorked)
                    .Where(
                        s => s.Hidden == false &&
                             s != ShiftRequest.RequestedShift &&
                             s != ShiftRequest.OriginalShift &&
                             s.Volunteer != null).ToListAsync();
        }

        private async Task<ShiftRequestAlert> LoadAlertForResolution(int alertId)
        {
            ShiftRequestAlert selectedAlert = await _context.ShiftAlerts.FirstOrDefaultAsync(p => p.Id == alertId);

            // load OriginalShift properties
            await _context.Entry(selectedAlert).Reference(p => p.OriginalShift).LoadAsync();
            await _context.Entry(selectedAlert.OriginalShift).Reference(p => p.PositionWorked).LoadAsync();
            await _context.Entry(selectedAlert.OriginalShift).Reference(p => p.Volunteer).LoadAsync();
            await _context.Entry(selectedAlert.OriginalShift.Volunteer).Reference(p => p.User).LoadAsync();
            await _context.Entry(selectedAlert.OriginalShift).Reference(p => p.ParentRecurringShift).LoadAsync();

            // load RequestedShift properties if its not null
            if (ShiftRequest.RequestedShift != null)
            {
                await _context.Entry(selectedAlert).Reference(p => p.RequestedShift).LoadAsync();
                await _context.Entry(selectedAlert.RequestedShift).Reference(p => p.PositionWorked).LoadAsync();
                await _context.Entry(selectedAlert.RequestedShift).Reference(p => p.Volunteer).LoadAsync();
                await _context.Entry(selectedAlert.RequestedShift).Reference(p => p.ParentRecurringShift).LoadAsync();

                if (selectedAlert.RequestedShift.Volunteer != null)
                {
                    await _context.Entry(selectedAlert.RequestedShift.Volunteer).Reference(p => p.User).LoadAsync();
                }
            }

            return selectedAlert;
        }

        private async Task<ShiftRequestAlert> LoadAlertForDisplay(int alertId)
        {
            ShiftRequestAlert shiftRequest = await _context.ShiftAlerts.FirstOrDefaultAsync(p => p.Id == alertId);
            await _context.Entry(shiftRequest).Reference(p => p.Volunteer).LoadAsync();
            await _context.Entry(shiftRequest).Reference(p => p.OriginalShift).LoadAsync();
            await _context.Entry(shiftRequest.OriginalShift).Reference(p => p.PositionWorked).LoadAsync();
            await _context.Entry(shiftRequest.OriginalShift).Reference(p => p.Volunteer).LoadAsync();
            await _context.Entry(shiftRequest).Reference(p => p.RequestedShift).LoadAsync();

            if (shiftRequest.RequestedShift != null)
            {
                await _context.Entry(shiftRequest.RequestedShift).Reference(p => p.PositionWorked).LoadAsync();
                await _context.Entry(shiftRequest.RequestedShift).Reference(p => p.Volunteer).LoadAsync();
            }

            await _context.Entry(shiftRequest).Reference(p => p.Volunteer).LoadAsync();

            return shiftRequest;
        }

        private void ResolveRecurringShiftDisplay()
        {
            bool isSwitchRequest = ShiftRequest.OriginalShift.ParentRecurringShift != null;

            _context.Entry(ShiftRequest.OriginalShift).Reference(p => p.ParentRecurringShift).LoadAsync();
            bool originalShiftIsPartOfRecurringSet = ShiftRequest.OriginalShift.ParentRecurringShift != null;

            bool requestedShiftIsPartOfRecurringSet = false;
            if (isSwitchRequest)
            {
                _context.Entry(ShiftRequest.RequestedShift).Reference(p => p.ParentRecurringShift).LoadAsync();
                requestedShiftIsPartOfRecurringSet = ShiftRequest.RequestedShift.ParentRecurringShift != null;
            }


            if (originalShiftIsPartOfRecurringSet)
            {
                ShiftRequest.RequestedShift.Hidden = false;

                DateTime selectedShiftDate = ShiftRequest.OriginalShift.StartDate;
                TimeSpan selectedShiftTime = ShiftRequest.OriginalShift.StartTime;

                DateTime combinedDateTime = new DateTime(
                    selectedShiftDate.Year,
                    selectedShiftDate.Month,
                    selectedShiftDate.Day,
                    selectedShiftTime.Hours,
                    selectedShiftTime.Minutes,
                    selectedShiftTime.Seconds);

                string excludedDateString = $"\\nEXDATE:{combinedDateTime.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}Z";

                // check both lists of shifts for the recurring shift that is the selected shift's parent
                bool recurringShiftIsOpen = OpenShifts.Any(s => s == ShiftRequest.OriginalShift.ParentRecurringShift);

                // once the recurring shift is found, exclude the selected shift from its set
                if (recurringShiftIsOpen)
                {
                    ((RecurringShift)OpenShifts.FirstOrDefault(s => s == ShiftRequest.OriginalShift.ParentRecurringShift)).RecurrenceRule += excludedDateString;
                }
                else
                {
                    ((RecurringShift)AssignedShifts.FirstOrDefault(s => s == ShiftRequest.OriginalShift.ParentRecurringShift)).RecurrenceRule += excludedDateString;
                }

                _context.Entry(ShiftRequest.OriginalShift.ParentRecurringShift).State = EntityState.Detached;
            }

            if (isSwitchRequest)
            {
                if (requestedShiftIsPartOfRecurringSet)
                {
                    ShiftRequest.RequestedShift.Hidden = false;

                    DateTime selectedShiftDate = ShiftRequest.RequestedShift.StartDate;
                    TimeSpan selectedShiftTime = ShiftRequest.RequestedShift.StartTime;

                    DateTime combinedDateTime = new DateTime(
                        selectedShiftDate.Year,
                        selectedShiftDate.Month,
                        selectedShiftDate.Day,
                        selectedShiftTime.Hours,
                        selectedShiftTime.Minutes,
                        selectedShiftTime.Seconds);

                    string excludedDateString = $"\\nEXDATE:{combinedDateTime.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}Z";

                    // check both lists of shifts for the recurring shift that is the selected shift's parent
                    bool recurringShiftIsOpen = OpenShifts.Any(s => s == ShiftRequest.RequestedShift.ParentRecurringShift);

                    // once the recurring shift is found, exclude the selected shift from its set
                    if (recurringShiftIsOpen)
                    {
                        ((RecurringShift)OpenShifts.FirstOrDefault(s => s == ShiftRequest.RequestedShift.ParentRecurringShift)).RecurrenceRule += excludedDateString;
                    }
                    else
                    {
                        ((RecurringShift)AssignedShifts.FirstOrDefault(s => s == ShiftRequest.RequestedShift.ParentRecurringShift)).RecurrenceRule += excludedDateString;
                    }

                    _context.Entry(ShiftRequest.RequestedShift.ParentRecurringShift).State = EntityState.Detached;
                }
            }
        }
    }
}