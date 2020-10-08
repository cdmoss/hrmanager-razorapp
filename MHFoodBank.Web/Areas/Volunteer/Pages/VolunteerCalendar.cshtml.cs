using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MHFoodBank.Common.Services;
using Syncfusion.EJ2;
using Syncfusion.EJ2.Base;

namespace MHFoodBank.Web.Areas.Volunteer.Pages
{
    [BindProperties]
    public class VolunteerCalendarModel : VolunteerPageModel
    {
        private readonly IMapper _mapper;
        public List<ShiftReadEditDto> AssignedShifts { get; set; }
        public List<ShiftReadEditDto> OpenShifts { get; set; }
        [BindProperty]
        public List<Position> Positions { get; set; }
        public bool AddedShift { get; set; } = false;
        public string StatusMessage { get; set; }
        [BindProperty]
        public int SelectedPositionId { get; set; }
        [BindProperty]
        public DateTime ClickedShiftDate { get; set; }
        [BindProperty]
        public ShiftReadEditDto SelectedShift { get; set; } = new ShiftReadEditDto();
        [BindProperty]
        public ApprovalStatus Approved { get; set; }

        public VolunteerCalendarModel(FoodBankContext context, UserManager<AppUser> userManager, IMapper mapper, string currentPage = "Schedule") : base(userManager, context, currentPage)
        {
            _mapper = mapper;
        }

        public async Task OnGet(string statusMessage = null)
        {
            StatusMessage = statusMessage;
            await PrepareModel();
        }

        public async Task<JsonResult> OnPostGetShifts([FromBody] CRUDModel model)
        {
            List<Shift> displayedShifts = new List<Shift>();

            if (model.Params.ContainsKey("type"))
            {
                if (model.Params["type"].ToString() == "open")
                {
                    var openShifts = _context.Shifts.Where(s => s.VolunteerProfileId == null).ToList();
                    displayedShifts = FilterShiftsByDate(openShifts);
                }
                else
                {
                    var user = await _userManager.GetUserAsync(User);
                    await _context.Entry(user).Reference(u => u.VolunteerProfile).LoadAsync();
                    await _context.Entry(user.VolunteerProfile).Collection(v => v.Shifts).LoadAsync();
                    var userShifts = user.VolunteerProfile.Shifts;
                    displayedShifts = FilterShiftsByDate(userShifts);
                }
            }
            else if (model.Params.ContainsKey("shiftId"))
            {
                var user = await _userManager.GetUserAsync(User);
                await _context.Entry(user).Reference(u => u.VolunteerProfile).LoadAsync();
                int shiftId = Convert.ToInt32(model.Params["shiftId"]);
                Shift selectedShift = await _context.Shifts.FirstOrDefaultAsync(s => s.Id == shiftId);
                if (model.Params.ContainsKey("startDate"))
                {
                    var selectedStartDate = ConvertSyncfusionDateStringToDateTime(model.Params["startDate"].ToString());
                    var excludedShift = new Shift()
                    {
                        StartTime = selectedStartDate,
                        EndTime = selectedStartDate + selectedShift.EndTime.Date.TimeOfDay,
                        IsAllDay = selectedShift.IsAllDay,
                        IsBlock = selectedShift.IsBlock,
                        Subject = selectedShift.Subject,
                        Position = selectedShift.Position,
                        Volunteer = user.VolunteerProfile,
                        RecurrenceID = selectedShift.Id
                    };

                    selectedShift.RecurrenceException += RecurrenceHelper.ConvertDateTimeToExDateString(selectedStartDate);
                    _context.Add(excludedShift);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    selectedShift.Volunteer = user.VolunteerProfile;
                }

                var openShifts = _context.Shifts.Where(s => s.VolunteerProfileId == null).ToList();
                displayedShifts = FilterShiftsByDate(openShifts);
            }   
            else
            {
                var user = await _userManager.GetUserAsync(User);
                await _context.Entry(user).Reference(u => u.VolunteerProfile).LoadAsync();
                await _context.Entry(user.VolunteerProfile).Collection(v => v.Shifts).LoadAsync();
                var userShifts = user.VolunteerProfile.Shifts;
                displayedShifts = FilterShiftsByDate(userShifts);
            }
            
            return new JsonResult(_mapper.Map<List<ShiftReadEditDto>>(displayedShifts));
        }

        private List<Shift> FilterShiftsByDate(IList<Shift> userShifts)
        {
            bool shiftShouldBeDisplayed;
            CurrentDateFilter filter = new CurrentDateFilter();
            var displayedShifts = new List<Shift>();
            // this foreach iterates through all the shifts and determines whether or not they should be displayed
            // and what color they should be displayed with (open vs assigned)
            foreach (var s in userShifts)
            {
                // CheckIfShiftDateIsAfterToday will handle recurring shifts in a special way:
                // it will check through all the shifts in it's recurrence set, if it finds one of the 
                // shifts to be scheduled past todays date, it will exclude all the shifts from that set
                // which are scheduled before todays date and display the rest
                shiftShouldBeDisplayed = filter.CheckIfShiftDateIsAfterToday(s);

                if (shiftShouldBeDisplayed)
                {
                    displayedShifts.Add(s);
                }
            }

            return displayedShifts;
        
        }

        private DateTime ConvertSyncfusionDateStringToDateTime(string inputString)
        {
            var splitInput = inputString.Split(' ');
            var month = splitInput[1];
            var day = Convert.ToInt32(splitInput[2]);
            var year = Convert.ToInt32(splitInput[3]);
            var time = splitInput[4];

            var splitTime = time.Split(':');
            var hour = Convert.ToInt32(splitTime[0]);
            var minute = Convert.ToInt32(splitTime[1]);
            var second = Convert.ToInt32(splitTime[2]);

            Dictionary<string, int> monthNumbers = new Dictionary<string, int>();
            monthNumbers.Add("Jan", 1);
            monthNumbers.Add("Feb", 2);
            monthNumbers.Add("Mar", 3);
            monthNumbers.Add("Apr", 4);
            monthNumbers.Add("May", 5);
            monthNumbers.Add("Jun", 6);
            monthNumbers.Add("Jul", 7);
            monthNumbers.Add("Aug", 8);
            monthNumbers.Add("Sep", 9);
            monthNumbers.Add("Oct", 10);
            monthNumbers.Add("Nov", 11);
            monthNumbers.Add("Dec", 12);

            int monthNumber = monthNumbers.GetValueOrDefault(month);
            return new DateTime(year, monthNumber, day, hour, minute, second);
        }

        private async Task<VolunteerProfile> PrepareModel()
        {
            var user = await _userManager.GetUserAsync(User);
            await _context.Entry(user).Reference(p => p.VolunteerProfile).LoadAsync();
            Positions = await _context.Positions.Where(p => p.Name != "All").ToListAsync();
            
            Approved = user.VolunteerProfile.ApprovalStatus;

            LoggedInUser = user.VolunteerProfile.FirstName + " " + user.VolunteerProfile.LastName;

            return user.VolunteerProfile;
        }
    }
}