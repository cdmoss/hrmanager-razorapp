using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using MHFoodBank.Web.Dtos;
using MHFoodBank.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Volunteer.Pages
{
    [BindProperties]
    public class RequestChangeModel : VolunteerPageModel
    {
        private readonly IMapper _mapper;

        public List<ShiftReadEditDto> AssignedShifts { get; set; }
        public List<ShiftReadEditDto> OpenShifts { get; set; }
        public ShiftReadEditDto OriginalShift { get; set; }
        public ShiftReadEditDto RequestedShift { get; set; }

        // these dates are used when shifts that are part of recurring shifts are selected as either the original or requested shift
        public DateTime OriginalIndividualDate { get; set; }
        public DateTime RequestedIndividualDate { get; set; }
        public string Reason { get; set; }
        public List<Position> Positions { get; set; }

        // these ids are passed via model binding once a requested shift is selected (removal requests only use OriginalShiftId)
        public int OriginalShiftId { get; set; }
        public int RequestedShiftId { get; set; }

        public RequestChangeModel(FoodBankContext context, UserManager<AppUser> userManager, IMapper mapper) : base(userManager, context)
        {
            _mapper = mapper;
        }

        public async Task OnGet(int originalShiftId, string originalShiftDate = null)
        {
            VolunteerProfile currentVolunteer = await PrepareModel(originalShiftId, originalShiftDate);
        }

        public async Task<IActionResult> OnPostChange()
        {
            // this method uses RequestedShiftId, OriginalShiftId and the userManager to build a shift request
            var newShiftRequest = await ConstructShiftRequestForSwitchAsync();

            _context.ShiftAlerts.Add(newShiftRequest);
            await _context.SaveChangesAsync();

            return RedirectToPage("/VolunteerCalendar", new { statusMessage = "You successfully requested to change your shift." });
        }

        public async Task<IActionResult> OnPostRemove()
        {
            var volunteer = (await _userManager.GetUserAsync(User)).VolunteerProfile;
            var originalShift = await _context.Shifts.FirstOrDefaultAsync(s => s.Id == OriginalShiftId);

            ShiftRequestAlert requestAlert = new ShiftRequestAlert()
            {
                Date = DateTime.Now,
                OriginalShift = originalShift,
                Reason = Reason,
                Volunteer = volunteer,
                Status = ShiftRequestAlert.RequestStatus.Pending
            };
            _context.ShiftAlerts.Add(requestAlert);
            await _context.SaveChangesAsync();

            return RedirectToPage("/VolunteerCalendar", new { statusMessage = "You successfully requested to remove your shift." });
        }

        private async Task<ShiftRequestAlert> ConstructShiftRequestForSwitchAsync()
        {
            // get volunteer
            var user = await _userManager.GetUserAsync(User);
            await _context.Entry(user).Reference(p => p.VolunteerProfile).LoadAsync();
            var volunteer = user.VolunteerProfile;
            // get requested shift
            var requestedShift = await _context.Shifts
                .Include(p => p.PositionWorked)
                .Include(p => p.Volunteer)
                .FirstOrDefaultAsync(s => s.Id == RequestedShiftId);
            // get original shift
            var originalShift = await _context.Shifts
                .Include(p => p.PositionWorked)
                .Include(p => p.Volunteer)
                .FirstOrDefaultAsync(s => s.Id == OriginalShiftId);

            // create skeleton of the alert, leave shifts null right now 
            // since if they are recurring shifts more processing must be done
            ShiftRequestAlert requestAlert = new ShiftRequestAlert()
            {
                Date = DateTime.Now,
                Reason = Reason,
                Volunteer = volunteer,
                Status = ShiftRequestAlert.RequestStatus.Pending
            };

            // if original shift is recurring shift, create a new shift with a date that matches the selected shift, hide it so it won't
            // display until the request is accepted
            if (originalShift is RecurringShift originalRecurringShift)
            {
                var selectedShiftFromRecurringSet = CreateIndividualShiftFromRecurringSet(OriginalIndividualDate, originalRecurringShift);

                // add created shift to the alert
                requestAlert.OriginalShift = selectedShiftFromRecurringSet;
            }
            else
            {
                // simply add the selected shift to the alert
                requestAlert.OriginalShift = originalShift;
            }

            // if requested shift is recurring shift, create a new shift with a date that matches the selected shift, hide it so it won't
            // display until the request is accepted
            if (requestedShift is RecurringShift newRecurringShift)
            {
                var selectedShiftFromRecurringSet = CreateIndividualShiftFromRecurringSet(RequestedIndividualDate, newRecurringShift);

                // add created shift to the alert
                requestAlert.RequestedShift = selectedShiftFromRecurringSet;
            }
            else
            {
                // simply add the selected shift to the alert
                requestAlert.RequestedShift = requestedShift;
            }
            return requestAlert;
        }

        private Shift CreateIndividualShiftFromRecurringSet(DateTime individualDate, RecurringShift recurringShift)
        {
            Shift selectedShiftFromRecurringSet = new Shift()
            {
                StartDate = individualDate,
                Hidden = true,
                StartTime = recurringShift.StartTime,
                EndTime = recurringShift.EndTime,
                ParentRecurringShift = recurringShift,
                Volunteer = recurringShift.Volunteer,
                PositionWorked = recurringShift.PositionWorked
            };
            selectedShiftFromRecurringSet.CreateDescription();

            return selectedShiftFromRecurringSet;
        }

        private async Task<VolunteerProfile> PrepareModel(int originalShiftId, string originalShiftDate = null)
        {
            AppUser currentUser = await _userManager.GetUserAsync(User);
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();

            var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            var allShifts = await _context.Shifts.Include(x => x.PositionWorked).Include(x => x.Volunteer).Where(x => x.Hidden == false).ToListAsync();
            var assignedShiftDomainModels = new List<Shift>();
            var openShiftDomainModels = new List<Shift>();
            bool shiftShouldBeDisplayed;

            CurrentDateFilter filter = new CurrentDateFilter();

            // this foreach iterates through all the shifts and determines whether or not they should be displayed
            // and what color they should be displayed with (open vs assigned)
            foreach (var s in allShifts)
            {

                // CheckIfShiftDateIsAfterToday will handle recurring shifts in a special way:
                // it will check through all the shifts in it's recurrence set, if it finds one of the 
                // shifts to be scheduled past todays date, it will exclude all the shifts from that set
                // which are scheduled before todays date and display the rest
                shiftShouldBeDisplayed = filter.CheckIfShiftDateIsAfterToday(s);

                if (shiftShouldBeDisplayed)
                {
                    bool shiftIsOpen = s.Volunteer == null;
                    if (shiftIsOpen)
                    {
                        openShiftDomainModels.Add(s);
                    }
                    else
                    {
                        bool showOthersShifts = s.Volunteer.Id != currentUser.VolunteerProfile.Id;
                        if (showOthersShifts)
                        {
                            assignedShiftDomainModels.Add(s);
                        }
                    }
                }
            }

            var mapper = new ShiftMapper(_mapper);

            AssignedShifts = mapper.MapShiftsToDtos(assignedShiftDomainModels);
            OpenShifts = mapper.MapShiftsToDtos(openShiftDomainModels);

            await _context.Entry(currentUser.VolunteerProfile).Collection(p => p.Shifts).LoadAsync();

            var originalShiftDomain = await _context.Shifts
                .Include(s => s.Volunteer)
                .Include(s => s.PositionWorked)
                .FirstOrDefaultAsync(s => s.Id == originalShiftId);

            OriginalShift = _mapper.Map<ShiftReadEditDto>(originalShiftDomain);

            // prevent duplicate display of shift if it's part of recurring shift
            if (originalShiftDate != null)
            {
                OriginalIndividualDate = Convert.ToDateTime(originalShiftDate);
            }
            else
            {
                OriginalIndividualDate = OriginalShift.StartDate;
            }

            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
            Positions = await _context.Positions.ToListAsync();

            return currentUser.VolunteerProfile;
        }
    }
}