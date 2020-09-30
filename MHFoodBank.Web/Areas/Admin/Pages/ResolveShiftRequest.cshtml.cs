using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire.Annotations;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
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
            //var requestAlert = await LoadAlertForDisplay(alertId);

            //ShiftRequest = _mapper.Map<ShiftRequestReadDto>(requestAlert);

            //if (ShiftRequest.Status == ShiftRequestAlert.RequestStatus.Pending)
            //{
            //    OpenShifts = await LoadOpenShifts(requestAlert);
            //    AssignedShifts = await LoadAssignedShifts(requestAlert);

            //    // if shift request is for a switch and the requested shift is part of a recurring set, 
            //    // this method will ensure that only one instance of that shift will be displayed
            //    ResolveRecurringShiftDisplay();
            //}
        }

        //public async Task<IActionResult> OnPostAccept(int alertId)
        //{
        //    var requestAlert = await LoadAlertForResolution(alertId);
        //    OpenShifts = await LoadOpenShifts(requestAlert); //*
        //    AssignedShifts = await LoadAssignedShifts(requestAlert); //*
        //    _context.Update(requestAlert);
        //    bool isRemovalRequest = requestAlert.RequestedShift == null;

        //    if (isRemovalRequest != true)
        //    {
        //        await ResolveSwitchRequest(requestAlert);
        //    }
        //    else
        //    {
        //        // the calendar only displays shifts whose hidden property == false so this alert won't be displayed
        //        // this is so the shift will be preserved and the alert can still be viewed in the volunteer's and admin's request archive
        //        // a better system may be worth creating to solve this problem
        //        requestAlert.OriginalShift.Hidden = true;

        //        var removedShift = requestAlert.OriginalShift;

        //        _reminderManager.CancelReminder(removedShift);
        //    }

        //    var alertsThatShareOriginalShift = _context.ShiftAlerts.Where(sa => sa.OriginalShift.Id == requestAlert.OriginalShift.Id);
        //    // decline each request that shares the same original shift
        //    foreach (var request in alertsThatShareOriginalShift)
        //    {
        //        _context.Update(request);
        //        request.Status = ShiftRequestAlert.RequestStatus.Declined;
        //        request.AddressedBy = User.Identity.Name;
        //        request.Read = true;
        //    }

        //    // this is to control the displayed status of the alert in the volunteer inbox
        //    requestAlert.Status = ShiftRequestAlert.RequestStatus.Accepted;
            
        //    await _context.SaveChangesAsync();

        //    return RedirectToPage("Alerts", new { statusMessage = "You accepted the shift change request" });
        //}

        //public async Task<IActionResult> OnPostDecline(int alertId)
        //{
        //    var requestAlert = await LoadAlertForResolution(alertId);
        //    _context.Update(requestAlert);
        //    // no changes to the requestalert's contents are made upon decline
        //    requestAlert.Status = ShiftRequestAlert.RequestStatus.Declined;
        //    requestAlert.AddressedBy = User.Identity.Name;
        //    await _context.SaveChangesAsync();
        //    return RedirectToPage("Alerts", new { statusMessage = "You declined the shift change request" });
        //}

        //private async Task ResolveSwitchRequest(ShiftRequestAlert requestAlert)
        //{
        //    // put all information that needs to be switched into external variables for readability
        //    var oldShiftVolunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == requestAlert.OriginalShift.Volunteer.Id);
        //    var oldShiftPosition = await _context.Positions.FirstOrDefaultAsync(v => v.Id == requestAlert.OriginalShift.PositionWorked.Id);
        //    var oldShiftTitleDescription = requestAlert.OriginalShift.Description;
        //    VolunteerProfile newShiftVolunteer = null;

        //    if (requestAlert.RequestedShift.Volunteer != null)
        //    {
        //        newShiftVolunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == requestAlert.RequestedShift.Volunteer.Id);
        //    }

        //    var newShiftPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == requestAlert.RequestedShift.PositionWorked.Id);
        //    var newShiftTitleDescription = requestAlert.RequestedShift.Description;

        //    // same with all the info that will stay the same
        //    var oldStartDate = requestAlert.OriginalShift.StartTime;
        //    var oldStartTime = requestAlert.OriginalShift.StartTime;
        //    var oldEndTime = requestAlert.OriginalShift.EndTime;
        //    var oldRecurringShift = requestAlert.OriginalShift.ParentRecurringShift;

        //    var newStartDate = requestAlert.RequestedShift.StartTime;
        //    var newStartTime = requestAlert.RequestedShift.StartTime;
        //    var newEndTime = requestAlert.RequestedShift.EndTime;
        //    var newRecurringShift = requestAlert.RequestedShift.ParentRecurringShift;

        //    // create two new shifts
        //    var updatedOriginalShift = new Shift();
        //    var updatedRequestedShift = new Shift();

        //    // switch volunteer, description of shifts, position worked
        //    updatedOriginalShift.Volunteer = newShiftVolunteer;
        //    updatedRequestedShift.Volunteer = oldShiftVolunteer;

        //    updatedOriginalShift.Description = newShiftTitleDescription;
        //    updatedRequestedShift.Description = oldShiftTitleDescription;

        //    updatedOriginalShift.PositionWorked = newShiftPosition;
        //    updatedRequestedShift.PositionWorked = oldShiftPosition;

        //    // fill in the rest of the information
        //    updatedOriginalShift.StartTime = oldStartDate;
        //    updatedOriginalShift.StartTime = oldStartTime;
        //    updatedOriginalShift.EndTime = oldEndTime;
        //    updatedOriginalShift.ParentRecurringShift = oldRecurringShift;

        //    updatedRequestedShift.StartTime = newStartDate;
        //    updatedRequestedShift.StartTime = newStartTime;
        //    updatedRequestedShift.EndTime = newEndTime;
        //    updatedRequestedShift.ParentRecurringShift = newRecurringShift;

        //    if (oldRecurringShift != null)
        //    {
        //        await UpdateExcludedShiftInRecurringSet(updatedOriginalShift, requestAlert.OriginalShift);
        //    }

        //    if (newRecurringShift != null)
        //    {
        //        await UpdateExcludedShiftInRecurringSet(updatedRequestedShift, requestAlert.RequestedShift);
        //    }
        //    // The reminders for the original shifts in this request must be cancelled before save changes is
        //    // called because in order for the reminder to be cancelled properly the original shifts parent
        //    // recurring shift must not be null
        //    CancelOldReminders(requestAlert);

        //    await _context.AddAsync(updatedOriginalShift);
        //    await _context.AddAsync(updatedRequestedShift);

        //    // hide old versions of shifts shifts
        //    requestAlert.OriginalShift.Hidden = true;
        //    requestAlert.RequestedShift.Hidden = true;

        //    requestAlert.AddressedBy = User.Identity.Name;

        //    await _context.SaveChangesAsync();

        //    // these are requests whose requested shift was the original shift of the request being changed. Their requested shift must be changed to 
        //    // the now updated request's updated original shift
        //    var requestsWithOrigAsRequested = _context.ShiftAlerts.Where(sa => sa.RequestedShift.Id == requestAlert.OriginalShift.Id);
        //    foreach (var request in requestsWithOrigAsRequested)
        //    {
        //        _context.Update(request);
        //        request.RequestedShift = updatedOriginalShift;
        //    }

        //    // these are requests whose requested shift was the requested shift of the request being changed. Their requested shift must be changed to 
        //    // the now updated request's updated requested shift
        //    var requestsWithRequestedAsRequested = _context.ShiftAlerts.Where(sa => sa.RequestedShift.Id == requestAlert.RequestedShift.Id);
        //    foreach (var request in requestsWithRequestedAsRequested)
        //    {
        //        _context.Update(request);
        //        request.RequestedShift = updatedRequestedShift;
        //    }

        //    await _context.SaveChangesAsync();

        //    // The reminders for the updated shifts must be scheduled after save changes is called
        //    // so the reminders have proper shift Ids
        //    ScheduleNewReminders(updatedOriginalShift, updatedRequestedShift);
        //}

        //private async Task UpdateExcludedShiftInRecurringSet(Shift newShift, Shift oldShift)
        //{
        //    // load recurring set's excluded shifts so the newshift can be added to it
        //    await _context.Entry(newShift.ParentRecurringShift).Collection(p => p.ExcludedShifts).LoadAsync();

        //    // exclude the new shift from the recurring set
        //    newShift.ParentRecurringShift.ExcludedShifts.Add(newShift);
        //    newShift.ParentRecurringShift.ExcludedShifts.Remove(oldShift);
        //    newShift.ParentRecurringShift.UpdateRecurrenceRule();
        //}
        
        //private void CancelOldReminders(ShiftRequestAlert requestAlert)
        //{
        //    // cancel old reminders; this must be done first. If reminders are scheduled first,
        //    // savechanges will be called and parentrecurring shifts for ShiftRequest.OriginalShift and ShiftRequest.RequestedShift will be null
        //    if (requestAlert.OriginalShift.Volunteer != null)
        //    {
        //        if (requestAlert.OriginalShift.ParentRecurringShift != null)
        //        {
        //            _reminderManager.CancelReminder(requestAlert.OriginalShift.ParentRecurringShift, requestAlert.OriginalShift.StartTime);
        //        }
        //        else
        //        {
        //            _reminderManager.CancelReminder(requestAlert.OriginalShift);
        //        }
        //    }

        //    if (requestAlert.RequestedShift.Volunteer != null)
        //    {
        //        if (requestAlert.RequestedShift.ParentRecurringShift != null)
        //        {
        //            _reminderManager.CancelReminder(requestAlert.RequestedShift.ParentRecurringShift, requestAlert.RequestedShift.StartTime);
        //        }
        //        else
        //        {
        //            _reminderManager.CancelReminder(requestAlert.RequestedShift);
        //        }
        //    }
        //}

        //private void ScheduleNewReminders(Shift OriginalShift, Shift RequestedShift)
        //{
        //    // schedule new reminders
        //    if (OriginalShift.Volunteer != null)
        //    {
        //        if (OriginalShift.ParentRecurringShift != null)
        //        {
        //            _reminderManager.ScheduleReminder(OriginalShift.Volunteer.User.Email, OriginalShift.Volunteer, OriginalShift, OriginalShift.StartTime);
        //        }
        //        else
        //        {
        //            _reminderManager.ScheduleReminder(OriginalShift.Volunteer.User.Email, OriginalShift.Volunteer, OriginalShift);
        //        }
        //    }

        //    if (RequestedShift.Volunteer != null)
        //    {
        //        if (RequestedShift.ParentRecurringShift != null)
        //        {
        //            _reminderManager.ScheduleReminder(RequestedShift.Volunteer.User.Email, RequestedShift.Volunteer, RequestedShift, RequestedShift.StartTime);
        //        }
        //        else
        //        {
        //            _reminderManager.ScheduleReminder(RequestedShift.Volunteer.User.Email, RequestedShift.Volunteer, RequestedShift);
        //        }
        //    }
        //}

        //private async Task<List<ShiftReadEditDto>> LoadOpenShifts(ShiftRequestAlert requestAlert)
        //{
        //    // get the requested shift id here so that the linq query doesnt break when it is a removal request(requested shift == null)
        //    int requestedShiftId = 0;
        //    if (requestAlert.RequestedShift != null)
        //    {
        //        requestedShiftId = requestAlert.RequestedShift.Id;
        //    }
            
        //    var domainShifts = await _context.Shifts
        //        .Include(p => p.Volunteer)
        //        .Include(p => p.PositionWorked)
        //        .Where(
        //            s => s.Hidden == false &&
        //                    s.Id != requestAlert.OriginalShift.Id &&
        //                    s.Id != requestedShiftId &&
        //                    s.Volunteer == null).ToListAsync();

        //    ShiftMapper sMapper = new ShiftMapper(_mapper);
        //    return sMapper.MapShiftsToDtos(domainShifts);
        //}

        //private async Task<List<ShiftReadEditDto>> LoadAssignedShifts(ShiftRequestAlert requestAlert)
        //{
        //    // get the requested shift id here so that the linq query doesnt break when it is a removal request(requested shift == null)
        //    int requestedShiftId = 0;
        //    if (requestAlert.RequestedShift != null)
        //    {
        //        requestedShiftId = requestAlert.RequestedShift.Id;
        //    }

        //    var domainShifts = await _context.Shifts
        //            .Include(p => p.Volunteer)
        //            .Include(p => p.PositionWorked)
        //            .Where(
        //                s => s.Hidden == false &&
        //                     s.Id != requestAlert.OriginalShift.Id &&
        //                     s.Id != requestedShiftId &&
        //                     s.Volunteer != null).ToListAsync();

        //    ShiftMapper sMapper = new ShiftMapper(_mapper);
        //    return sMapper.MapShiftsToDtos(domainShifts);
        //}

        //private async Task<ShiftRequestAlert> LoadAlertForResolution(int alertId)
        //{
        //    var shiftRequest = await _context.ShiftAlerts
        //        .Include(p => p.OriginalShift).ThenInclude(p => p.PositionWorked)
        //        .Include(p => p.OriginalShift).ThenInclude(p => p.Volunteer).ThenInclude(p => p.User)
        //        .Include(p => p.OriginalShift).ThenInclude(p => p.ParentRecurringShift)
        //        .Include(p => p.RequestedShift).ThenInclude(p => p.PositionWorked)
        //        .Include(p => p.RequestedShift).ThenInclude(p => p.Volunteer).ThenInclude(p => p.User)
        //        .Include(p => p.RequestedShift).ThenInclude(p => p.ParentRecurringShift)
        //        .FirstOrDefaultAsync(p => p.Id == alertId);

        //    return shiftRequest;
        //}

        //private async Task<ShiftRequestAlert> LoadAlertForDisplay(int alertId)
        //{
        //    var domainShiftRequest = await _context.ShiftAlerts
        //        .Include(p => p.Volunteer)
        //        .Include(p => p.OriginalShift).ThenInclude(p => p.PositionWorked)
        //        .Include(p => p.OriginalShift).ThenInclude(p => p.Volunteer)
        //        .Include(p => p.OriginalShift).ThenInclude(p => p.ParentRecurringShift)
        //        .Include(p => p.RequestedShift).ThenInclude(p => p.ParentRecurringShift)
        //        .Include(p => p.RequestedShift).ThenInclude(p => p.Volunteer)
        //        .Include(p => p.RequestedShift).ThenInclude(p => p.PositionWorked)
        //        .FirstOrDefaultAsync(p => p.Id == alertId);

        //    return domainShiftRequest;
        //}

        //private void ResolveRecurringShiftDisplay()
        //{
        //    bool isSwitchRequest = ShiftRequest.RequestedShift != null;
        //    bool originalShiftIsPartOfRecurringSet = ShiftRequest.OriginalShift.ParentRecurringShiftId > 0;
        //    bool requestedShiftIsPartOfRecurringSet = false;

        //    if (isSwitchRequest)
        //    {
        //        requestedShiftIsPartOfRecurringSet = ShiftRequest.RequestedShift.ParentRecurringShiftId > 0;
        //    }

        //    if (originalShiftIsPartOfRecurringSet)
        //    {
        //        ExcludeShiftFromRecurringSet(ShiftRequest.OriginalShift, OpenShifts, AssignedShifts);
        //    }

        //    if (isSwitchRequest)
        //    {
        //        if (requestedShiftIsPartOfRecurringSet)
        //        {
        //            ExcludeShiftFromRecurringSet(ShiftRequest.RequestedShift, OpenShifts, AssignedShifts);
        //        }
        //    }
        //}

        //// this method will inspect given 2 lists of shifts for a recurring set of which
        //// the given excluded shift is a part of, 
        //// once it finds the recurring shift it will then 
        //// exclude the given shift from the found set
        //public static void ExcludeShiftFromRecurringSet(
        //    ShiftReadEditDto excludedShift,
        //    List<ShiftReadEditDto> openShifts,
        //    List<ShiftReadEditDto> assignedShifts)
        //{
        //    DateTime selectedShiftDate = excludedShift.StartDate;
        //    TimeSpan selectedShiftTime = excludedShift.StartTime;

        //    DateTime combinedDateTime = new DateTime(
        //        selectedShiftDate.Year,
        //        selectedShiftDate.Month,
        //        selectedShiftDate.Day,
        //        selectedShiftTime.Hours,
        //        selectedShiftTime.Minutes,
        //        selectedShiftTime.Seconds);

        //    string excludedDateString = $"\\nEXDATE:{combinedDateTime.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}Z";

        //    // check both lists of shifts for the recurring shift that is the selected shift's parent
        //    bool recurringShiftIsOpen = openShifts.Any(s => s.Id == excludedShift.ParentRecurringShiftId);

        //    // once the recurring shift is found, exclude the selected shift from its set
        //    if (recurringShiftIsOpen)
        //    {
        //        openShifts
        //            .FirstOrDefault(s => s.Id == excludedShift.ParentRecurringShiftId).RecurrenceRule += excludedDateString;
        //    }
        //    else
        //    {
        //        assignedShifts
        //            .FirstOrDefault(s => s.Id == excludedShift.ParentRecurringShiftId).RecurrenceRule += excludedDateString;
        //    }
        //}
    }
}