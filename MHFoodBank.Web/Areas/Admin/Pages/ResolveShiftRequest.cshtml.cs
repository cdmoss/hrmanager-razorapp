using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire.Annotations;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Data.Models;
using MHFoodBank.Web.Dtos;
using MHFoodBank.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin")]
    public class ResolveShiftRequestModel : AdminPageModel
    {
        private readonly IReminderManager _reminderManager;
        private readonly IMapper _mapper;
        public enum RequestResolvePageViewType
        {
            NewSwitch,
            NewRemoval,
            ArchivedSwitch,
            ArchivedRemoval
        }

        // when ShiftRequest.RequestedShift is null it means its a request to remove shift entirely
        public ShiftRequestReadDto ShiftRequest { get; set; }
        public List<ShiftReadEditDto> OpenShifts { get; set; }
        public List<ShiftReadEditDto> AssignedShifts { get; set; }
        public RequestResolvePageViewType ViewType { get; set; }

        public ResolveShiftRequestModel(FoodBankContext context, IReminderManager reminderManager, IMapper mapper, string currentPage = "Shift Request") : base(context, currentPage)
        {
            _reminderManager = reminderManager;
            _mapper = mapper;
        }

        public async Task OnGet(int alertId, string requestType)
        {
            var requestAlert = await LoadAlertForDisplay(alertId);

            ShiftRequest = _mapper.Map<ShiftRequestReadDto>(requestAlert);

            if (ShiftRequest.Status == ShiftRequestAlert.RequestStatus.Pending)
            {
                OpenShifts = await LoadOpenShifts(requestAlert);
                AssignedShifts = await LoadAssignedShifts(requestAlert);
            }
            // if shift request is for a switch and the requested shift is part of a recurring set, 
            // this method will ensure that only one instance of that shift will be displayed
            ResolveRecurringShiftDisplay();
        }

        public async Task<IActionResult> OnPostAccept(int alertId)
        {
            var requestAlert = await LoadAlertForResolution(alertId);
            OpenShifts = await LoadOpenShifts(requestAlert);
            AssignedShifts = await LoadAssignedShifts(requestAlert);
            _context.Update(requestAlert);
            bool isRemovalRequest = requestAlert.RequestedShift == null;

            if (isRemovalRequest != true)
            {
                await ResolveSwitchRequest(requestAlert);
            }
            else
            {
                // the calendars only display shifts whose archived property == false so this alert won't be displayed
                // this is so the shift will be preserved and the alert can still be viewed in the volunteer's and admin's request archive
                // a better system may be worth creating to solve this problem
                requestAlert.OriginalShift.Hidden = true;

                var removedShift = requestAlert.OriginalShift;

                _reminderManager.CancelReminder(removedShift);
            }

            // this is to control the displayed status of the alert in the volunteer inbox
            requestAlert.Status = ShiftRequestAlert.RequestStatus.Accepted;
            
            await _context.SaveChangesAsync();

            return RedirectToPage("Alerts", new { statusMessage = "You accepted the shift change request" });
        }

        public async Task<IActionResult> OnPostDecline(int alertId)
        {
            var requestAlert = await LoadAlertForResolution(alertId);
            _context.Update(requestAlert);
            // no changes to the requestalert's contents are made upon decline
            requestAlert.Status = ShiftRequestAlert.RequestStatus.Declined;
            await _context.SaveChangesAsync();
            return RedirectToPage("Alerts", new { statusMessage = "You declined the shift change request" });
        }

        private async Task ResolveSwitchRequest(ShiftRequestAlert requestAlert)
        {
            // put all information that needs to be switched into external variables for readability
            var oldShiftVolunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == requestAlert.OriginalShift.Volunteer.Id);
            var oldShiftPosition = await _context.Positions.FirstOrDefaultAsync(v => v.Id == requestAlert.OriginalShift.PositionWorked.Id);
            var oldShiftTitleDescription = requestAlert.OriginalShift.Description;
            VolunteerProfile newShiftVolunteer = null;

            if (requestAlert.RequestedShift.Volunteer != null)
            {
                newShiftVolunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == requestAlert.RequestedShift.Volunteer.Id);
            }

            var newShiftPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == requestAlert.RequestedShift.PositionWorked.Id);
            var newShiftTitleDescription = requestAlert.RequestedShift.Description;

            // same with all the info that will stay the same
            var oldStartDate = requestAlert.OriginalShift.StartDate;
            var oldStartTime = requestAlert.OriginalShift.StartTime;
            var oldEndTime = requestAlert.OriginalShift.EndTime;
            var oldRecurringShift = requestAlert.OriginalShift.ParentRecurringShift;

            var newStartDate = requestAlert.RequestedShift.StartDate;
            var newStartTime = requestAlert.RequestedShift.StartTime;
            var newEndTime = requestAlert.RequestedShift.EndTime;
            var newRecurringShift = requestAlert.RequestedShift.ParentRecurringShift;

            // create two new shifts
            var updatedOriginalShift = new Shift();
            var updatedRequestedShift = new Shift();

            // switch volunteer, description of shifts, position worked
            updatedOriginalShift.Volunteer = newShiftVolunteer;
            updatedRequestedShift.Volunteer = oldShiftVolunteer;

            updatedOriginalShift.Description = newShiftTitleDescription;
            updatedRequestedShift.Description = oldShiftTitleDescription;

            updatedOriginalShift.PositionWorked = newShiftPosition;
            updatedRequestedShift.PositionWorked = oldShiftPosition;

            // fill in the rest of the information
            updatedOriginalShift.StartDate = oldStartDate;
            updatedOriginalShift.StartTime = oldStartTime;
            updatedOriginalShift.EndTime = oldEndTime;
            updatedOriginalShift.ParentRecurringShift = oldRecurringShift;

            updatedRequestedShift.StartDate = newStartDate;
            updatedRequestedShift.StartTime = newStartTime;
            updatedRequestedShift.EndTime = newEndTime;
            updatedRequestedShift.ParentRecurringShift = newRecurringShift;

            if (oldRecurringShift != null)
            {
                // load recurring set's excluded shifts so the newshift can be added to it
                await _context.Entry(updatedOriginalShift.ParentRecurringShift).Collection(p => p.ExcludedShifts).LoadAsync();

                // exclude the new shift from the recurring set and remove the old version 
                updatedOriginalShift.ParentRecurringShift.ExcludedShifts.Add(updatedOriginalShift);
                updatedOriginalShift.ParentRecurringShift.ExcludedShifts.Remove(requestAlert.OriginalShift);
                updatedOriginalShift.ParentRecurringShift.UpdateRecurrenceRule();
            }

            if (newRecurringShift != null)
            {
                // load recurring set's excluded shifts so the newshift can be added to it
                await _context.Entry(updatedRequestedShift.ParentRecurringShift).Collection(p => p.ExcludedShifts).LoadAsync();

                // exclude the new shift from the recurring set
                updatedRequestedShift.ParentRecurringShift.ExcludedShifts.Add(updatedRequestedShift);
                updatedRequestedShift.ParentRecurringShift.ExcludedShifts.Remove(requestAlert.RequestedShift);
                updatedRequestedShift.ParentRecurringShift.UpdateRecurrenceRule();
            }

            await _context.AddAsync(updatedOriginalShift);
            await _context.AddAsync(updatedRequestedShift);

            // hide old versions of shifts shifts
            requestAlert.OriginalShift.Hidden = true;
            requestAlert.RequestedShift.Hidden = true;

            requestAlert.AddressedBy = User.Identity.Name;

            await _context.SaveChangesAsync();

            // schedule reminders, this must be done while the old shifts still have parentrecurringshifts
            ScheduleRemindersAfterSwitch(updatedOriginalShift, updatedRequestedShift, requestAlert);

        }
        
        private void ScheduleRemindersAfterSwitch(Shift OriginalShift, Shift RequestedShift, ShiftRequestAlert requestAlert)
        {
            // cancel old reminders; this must be done first, if reminders are scheduled first, 
            // savechanges will be called and parentrecurring shifts will be for ShiftRequest.OriginalShift and ShiftRequest.RequestedShift will be null
            if (requestAlert.OriginalShift.Volunteer != null)
            {
                if (requestAlert.OriginalShift.ParentRecurringShift != null)
                {
                    _reminderManager.CancelReminder(requestAlert.OriginalShift.ParentRecurringShift, requestAlert.OriginalShift.StartDate);
                }
                else
                {
                    _reminderManager.CancelReminder(requestAlert.OriginalShift);
                }
            }

            if (requestAlert.RequestedShift.Volunteer != null)
            {
                if (requestAlert.RequestedShift.ParentRecurringShift != null)
                {
                    _reminderManager.CancelReminder(requestAlert.RequestedShift.ParentRecurringShift, requestAlert.RequestedShift.StartDate);
                }
                else
                {
                    _reminderManager.CancelReminder(requestAlert.RequestedShift);
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

        private async Task<List<ShiftReadEditDto>> LoadOpenShifts(ShiftRequestAlert requestAlert)
        {
            // get the requested shift id here so that the linq query doesnt break when it is a removal request(requested shift == null)
            int requestedShiftId = 0;
            if (requestAlert.RequestedShift != null)
            {
                requestedShiftId = requestAlert.RequestedShift.Id;
            }
            
            var domainShifts = await _context.Shifts
                .Include(p => p.Volunteer)
                .Include(p => p.PositionWorked)
                .Where(
                    s => s.Hidden == false &&
                            s.Id != requestAlert.OriginalShift.Id &&
                            s.Id != requestedShiftId &&
                            s.Volunteer == null).ToListAsync();

            ShiftMapper sMapper = new ShiftMapper(_mapper);
            return sMapper.MapShiftsToDtos(domainShifts);
        }

        private async Task<List<ShiftReadEditDto>> LoadAssignedShifts(ShiftRequestAlert requestAlert)
        {
            // get the requested shift id here so that the linq query doesnt break when it is a removal request(requested shift == null)
            int requestedShiftId = 0;
            if (requestAlert.RequestedShift != null)
            {
                requestedShiftId = requestAlert.RequestedShift.Id;
            }

            var domainShifts = await _context.Shifts
                    .Include(p => p.Volunteer)
                    .Include(p => p.PositionWorked)
                    .Where(
                        s => s.Hidden == false &&
                             s.Id != requestAlert.OriginalShift.Id &&
                             s.Id != requestedShiftId &&
                             s.Volunteer != null).ToListAsync();

            ShiftMapper sMapper = new ShiftMapper(_mapper);
            return sMapper.MapShiftsToDtos(domainShifts);
        }

        private async Task<ShiftRequestAlert> LoadAlertForResolution(int alertId)
        {
            ShiftRequestAlert selectedAlert = await _context.ShiftAlerts.FirstOrDefaultAsync(p => p.Id == alertId);

            // load oldshift properties
            await _context.Entry(selectedAlert).Reference(p => p.OriginalShift).LoadAsync();
            await _context.Entry(selectedAlert.OriginalShift).Reference(p => p.PositionWorked).LoadAsync();
            await _context.Entry(selectedAlert.OriginalShift).Reference(p => p.Volunteer).LoadAsync();
            await _context.Entry(selectedAlert.OriginalShift.Volunteer).Reference(p => p.User).LoadAsync();
            await _context.Entry(selectedAlert.OriginalShift).Reference(p => p.ParentRecurringShift).LoadAsync();

            // load newshift properties if its not null
            if (selectedAlert.RequestedShift != null)
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
            var domainShiftRequest = await _context.ShiftAlerts
                .Include(p => p.Volunteer)
                .Include(p => p.OriginalShift).ThenInclude(p => p.PositionWorked)
                .Include(p => p.OriginalShift).ThenInclude(p => p.Volunteer)
                .Include(p => p.RequestedShift)
                .FirstOrDefaultAsync(p => p.Id == alertId);

            if (domainShiftRequest.RequestedShift != null)
            {
                await _context.Entry(domainShiftRequest.RequestedShift).Reference(p => p.PositionWorked).LoadAsync();
                await _context.Entry(domainShiftRequest.RequestedShift).Reference(p => p.Volunteer).LoadAsync();
            }

            return domainShiftRequest;
        }

        private void ResolveRecurringShiftDisplay()
        {
            bool isSwitchRequest = ShiftRequest.RequestedShift != null;
            bool originalShiftIsPartOfRecurringSet = ShiftRequest.OriginalShift.ParentRecurringShiftId > 0;
            bool requestedShiftIsPartOfRecurringSet = false;

            if (isSwitchRequest)
            {
                requestedShiftIsPartOfRecurringSet = ShiftRequest.RequestedShift.ParentRecurringShiftId > 0; ;
            }

            if (originalShiftIsPartOfRecurringSet)
            {
                ShiftRequest.RequestedShift.Hidden = false;

                DateTime selectedShiftDate = ShiftRequest.OriginalShift.StartDate;
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
                bool recurringShiftIsOpen = OpenShifts.Any(s => s.ParentRecurringShiftId == ShiftRequest.OriginalShift.ParentRecurringShiftId);

                // once the recurring shift is found, exclude the selected shift from its set
                if (recurringShiftIsOpen)
                {
                    OpenShifts
                        .FirstOrDefault(s => s.ParentRecurringShiftId == ShiftRequest.OriginalShift.ParentRecurringShiftId).RecurrenceRule += excludedDateString;
                }
                else
                {
                    AssignedShifts
                        .FirstOrDefault(s => s.ParentRecurringShiftId == ShiftRequest.OriginalShift.ParentRecurringShiftId).RecurrenceRule += excludedDateString;
                }

                _context.Entry(ShiftRequest.OriginalShift.ParentRecurringShiftId).State = EntityState.Detached;
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
                    bool recurringShiftIsOpen = OpenShifts.Any(s => s.ParentRecurringShiftId == ShiftRequest.RequestedShift.ParentRecurringShiftId);

                    // once the recurring shift is found, exclude the selected shift from its set
                    if (recurringShiftIsOpen)
                    {
                        OpenShifts.FirstOrDefault(s => s.ParentRecurringShiftId == ShiftRequest.RequestedShift.ParentRecurringShiftId).RecurrenceRule += excludedDateString;
                    }
                    else
                    {
                        AssignedShifts.FirstOrDefault(s => s.ParentRecurringShiftId == ShiftRequest.RequestedShift.ParentRecurringShiftId).RecurrenceRule += excludedDateString;
                    }
                }
            }
        }
    }
}