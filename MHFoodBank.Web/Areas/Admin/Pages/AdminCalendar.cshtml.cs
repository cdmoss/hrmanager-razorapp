using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Hangfire.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using MHFoodBank.Common.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using MHFoodBank.Web.Services;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Text.Json;
using Syncfusion.EJ2.Base;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin")]
    [BindProperties]
    public class AdminCalendar : AdminPageModel
    {
        private readonly IReminderManager _reminderManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        public enum RecurringShiftEditType
        {
            Single,
            WholeSet
        }

        #region model properties
        // for choosing a volunteer when editing/adding a shift
        public List<Position> PositionsWithAll { get; set; }
        public List<Position> Positions { get; set; }
        public string[] ResourceNames { get; set; } = new string[] { "Positions" };
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        public string SearchedName { get; set; }
        public int SearchedPositionId { get; set; }
        // position that was selected in the edit/delete position window
        public string SelectedPositionName { get; set; }
        public string SelectedPositionColor { get; set; }
        public string NewPositionColor { get; set; }
        public string NewPositionName { get; set; }
        // give user feedback after action
        public string StatusMessage { get; set; }
        [BindProperty(Name = nameof(ShiftAmounts))] public Dictionary<int, int> ShiftAmounts { get; set; }
        // to change behaviour of recurring shift edit process, 
        // switches between editing only the selected shift or the whole recurring set
        public RecurringShiftEditType EditType { get; set; }
        #region weekdays for recurring shift add/edit window
        // these are bound to the checkboxes on the recurring shift add/edit window
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        #endregion

        // populates the calendar with shifts currently in the db
        // Testing to see if we can just use recurring shift read edit dtos
        #endregion

        private readonly IMapper _mapper;

        public AdminCalendar(FoodBankContext context, IEmailSender emailSender, UserManager<AppUser> userManager, IMapper mapper, IReminderManager reminderManager, string currentPage = "Scheduling") : base(context, currentPage)
        {
            _mapper = mapper;
            _reminderManager = reminderManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task OnGet()
        {
            await PrepareModel(null);
        }

        private async Task PrepareModel(string statusMessage)
        {
            // get shifts, recurring shifts and volunteers in domain model form then map them to dtos
            var volunteerDomainModels = await _context.VolunteerProfiles
                .Include(p => p.Positions)
                .Where(v => v != null && v.ApprovalStatus == ApprovalStatus.Approved).ToListAsync();
            var shiftDomainModels = await _context.Shifts
                .Include(p => p.Volunteer).ThenInclude(v => v.Availabilities)
                .Include(p => p.Position).ToListAsync();

            ShiftMapper map = new ShiftMapper(_mapper);
            //Shifts = map.MapShiftsToDtos(shiftDomainModels);

            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);

            // get positions
            Positions = _context.Positions.Where(p => !p.Deleted && p.Name != "All").OrderBy(p => p.Name).ToList();
            PositionsWithAll = _context.Positions.Where(p => !p.Deleted).OrderBy(p => p.Name).ToList();
            SearchedPositionId = PositionsWithAll.FirstOrDefault(p => p.Name == "All").Id;

            ShiftAmounts = new Dictionary<int, int>();

            foreach (var position in Positions)
            {
                ShiftAmounts.Add(position.Id, 0);
            }

            // update status message
            StatusMessage = statusMessage;
        }

        public JsonResult OnPostGetShifts()
        {
            var shifts = _context.Shifts.ToList();
            return new JsonResult(_mapper.Map<List<ShiftReadEditDto>>(shifts));
        }

        public async Task<JsonResult> OnPostInsertShift([FromBody] CRUDModel<ShiftReadEditDto> shiftDto)
        {
            var newShiftDto = (shiftDto.Action == "insert") ? shiftDto.Value : shiftDto.Added[0];
            newShiftDto.StartTime = newShiftDto.StartTime.AddHours(-6);
            newShiftDto.EndTime = newShiftDto.EndTime.AddHours(-6);
            int intMax = _context.Shifts.ToList().Count > 0 ? _context.Shifts.ToList().Max(p => p.Id) : 1;
            var newShift = _mapper.Map<Shift>(newShiftDto);
            _context.Shifts.Add(newShift);
            await _context.SaveChangesAsync();
            var data = await _context.Shifts.ToListAsync();
            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostUpdateShifts([FromBody] CRUDModel<ShiftReadEditDto> shiftDto)
        {
            if (shiftDto != null)
            {
                if (shiftDto.Action == "insert" || (shiftDto.Action == "batch" && shiftDto.Added.Count > 0)) // this block of code will execute while inserting the appointments
                {
                    var newShiftDto = (shiftDto.Action == "insert") ? shiftDto.Value : shiftDto.Added[0];
                    newShiftDto.Id = 0;
                    newShiftDto.StartTime = newShiftDto.StartTime.AddHours(-6);
                    newShiftDto.EndTime = newShiftDto.EndTime.AddHours(-6);
                    var newShift = _mapper.Map<Shift>(newShiftDto);
                    if (newShift.VolunteerProfileId == 0)
                    {
                        newShift.VolunteerProfileId = null;
                    }

                    bool multipleShifts = Convert.ToBoolean(shiftDto.Params["multiShifts"]);
                    if (multipleShifts)
                    {
                        var positions = _context.Positions.Where(p => p.Name != "All").ToList();
                        foreach (var pos in positions)
                        {
                            int positionAmount = Convert.ToInt32(shiftDto.Params[$"{pos.Name}-amount"]);
                            for (int i = 0; i < positionAmount; i++)
                            {
                                newShift.Id = 0;
                                newShift.Position = pos;

                                var chosenPosition = pos;
                                var chosenVolunteer = newShift.VolunteerProfileId != null ? await _context.VolunteerProfiles
                                    .Where(p => p.Id == newShift.VolunteerProfileId).FirstOrDefaultAsync() : null;

                                newShift.Subject = newShift.VolunteerProfileId == null ?
                                    $"Open - {chosenPosition.Name}" :
                                    $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

                                _context.Shifts.Add(newShift);

                                await _context.SaveChangesAsync();

                                // schedule email notification for shift
                                if (newShift.VolunteerProfileId != null)
                                {
                                    var volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == newShift.VolunteerProfileId);
                                    if(string.IsNullOrEmpty(newShift.RecurrenceRule))
                                    {
                                        await _reminderManager.ScheduleReminder(volunteerAccount, newShift);
                                    }
                                    else
                                    {
                                        await _reminderManager.ScheduleReminder(volunteerAccount, newShift);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var chosenPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == newShiftDto.PositionId);
                        var chosenVolunteer = newShift.VolunteerProfileId != null ? await _context.VolunteerProfiles
                            .Where(p => p.Id == newShift.VolunteerProfileId).FirstOrDefaultAsync() : null;

                        newShift.Subject = newShift.VolunteerProfileId == null ?
                            $"Open - {chosenPosition.Name}" :
                            $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

                        _context.Shifts.Add(newShift);

                        await _context.SaveChangesAsync();

                        // schedule email notification for shift
                        if (newShift.VolunteerProfileId != null)
                        {
                            var volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == newShift.VolunteerProfileId);
                            await _reminderManager.ScheduleReminder(volunteerAccount, newShift);
                        }
                    }


                }
                if (shiftDto.Action == "update" || (shiftDto.Action == "batch" && shiftDto.Changed.Count > 0)) // this block of code will execute while updating the appointment
                {
                    var value = (shiftDto.Action == "update") ? shiftDto.Value : shiftDto.Changed[0];
                    var newShift = await _context.Shifts.FirstOrDefaultAsync(c => c.Id == Convert.ToInt32(value.Id));
                    _reminderManager.CancelReminder(newShift);
                    if (newShift != null)
                    {
                        newShift.StartTime = value.StartTime.AddHours(-6);
                        newShift.EndTime = value.EndTime.AddHours(-6);
                        newShift.PositionId = value.PositionId;
                        newShift.VolunteerProfileId = value.VolunteerProfileId;
                        newShift.IsAllDay = value.IsAllDay;
                        newShift.RecurrenceRule = value.RecurrenceRule;
                        newShift.RecurrenceID = value.RecurrenceID;
                        newShift.RecurrenceException = value.RecurrenceException;
                        if (newShift.VolunteerProfileId == 0)
                        {
                            newShift.VolunteerProfileId = null;
                        }

                        var chosenPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == newShift.PositionId);
                        var chosenVolunteer = newShift.VolunteerProfileId != null ? await _context.VolunteerProfiles
                            .FirstOrDefaultAsync(p => p.Id == newShift.VolunteerProfileId) : null;

                        newShift.Subject = newShift.VolunteerProfileId == null ?
                        $"Open - {chosenPosition.Name}" :
                            $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

                        await _context.SaveChangesAsync();

                        await _context.Entry(chosenVolunteer.User).Reference(u => u).LoadAsync();
                        await _reminderManager.ScheduleReminder(chosenVolunteer.User, newShift);
                    }
                }
                if (shiftDto.Action == "remove" || (shiftDto.Action == "batch" && shiftDto.Deleted.Count > 0)) // this block of code will execute while removing the appointment
                {
                    if (shiftDto.Action == "remove")
                    {
                        int key = Convert.ToInt32(shiftDto.Key);
                        var newShift = await _context.Shifts.Where(c => c.Id == key).FirstOrDefaultAsync();
                        if (newShift != null)
                        {
                            _reminderManager.CancelReminder(newShift);
                            _context.Shifts.Remove(newShift);
                        }
                    }
                    else
                    {
                        foreach (var shifts in shiftDto.Deleted)
                        {
                            var newShift = await _context.Shifts.Where(c => c.Id == shifts.Id).FirstOrDefaultAsync();
                            if (shifts != null)
                            {
                                _reminderManager.CancelReminder(newShift);
                                _context.Shifts.Remove(newShift);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                };
            }

            var shiftDomainModels = await _context.Shifts.ToListAsync();
            return new JsonResult(shiftDomainModels);
        }

        public async Task OnPostSearch()
        {
            //store in local variable so it doesn't get overwritten by prepare model
            int searchedPosId = SearchedPositionId;
            await PrepareModel(null);

            var searcher = new Searcher(_context);
            var searchedPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Id == searchedPosId);
            //Shifts = searcher.FilterShiftsBySearch(Shifts, SearchedName, searchedPosition);
        }

        public async Task<IActionResult> OnPostAddPosition()
        {
            // get entered name and check if a position with that name exists

            bool positionAlreadyExists = _context.Positions.Any(p => string.Equals(NewPositionName, p.Name));
            bool noPositionNameEntered = string.IsNullOrWhiteSpace(NewPositionName);

            if (positionAlreadyExists)
            {
                return RedirectToPage(new { statusMessage = $"Error: A position with that name already exists." });
            }
            else if (noPositionNameEntered)
            {
                return RedirectToPage(new { statusMessage = $"Error: You must enter a name for the position." });
            }

            Position position = new Position { Name = NewPositionName, Color = NewPositionColor };
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You successfully added {position.Name} to the list of positions." });
            return null;
        }

        public async Task<IActionResult> OnPostEditPosition()
        {
            bool positionAlreadyExists = await _context.Positions.AnyAsync(p => string.Equals(NewPositionName, p.Name));
            bool noPositionNameEntered = string.IsNullOrWhiteSpace(NewPositionName);

            if (positionAlreadyExists)
            {
                return RedirectToPage(new { statusMessage = $"Error: A position with that name already exists." });
            }
            else if (noPositionNameEntered)
            {
                return RedirectToPage(new { statusMessage = $"Error: You must enter a name for the position." });
            }

            var selectedPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Name == SelectedPositionName);

            string resultStatus = await EditPosition(selectedPosition);
            return RedirectToPage(new { statusMessage = resultStatus });
            return null;
        }

        public async Task<IActionResult> OnPostRemovePosition()
        {
            var selectedPosition = _context.Positions.FirstOrDefault(p => p.Name == SelectedPositionName);
            string resultStatus = await RemovePosition(selectedPosition);
            return RedirectToPage(new { statusMessage = resultStatus });
        }

        private async Task<string> EditPosition(Position selectedPosition)
        {
            if (selectedPosition != null)
            {
                string originalName = selectedPosition.Name;

                _context.Update(selectedPosition);

                selectedPosition.Name = SelectedPositionName;
                selectedPosition.Color = SelectedPositionColor;
                await _context.SaveChangesAsync();
                return $"You successfully updated {originalName} to {selectedPosition.Name}.";
            }
            return $"Error: You must select a position.";
        }

        private async Task<string> RemovePosition(Position selectedPosition)
        {
            if (selectedPosition != null)
            {
                var positionVolunteers = await _context.PositionVolunteers.Where(s => s.Position == selectedPosition).ToListAsync();
                var shiftsWithPosition = await _context.Shifts.Where(s => s.Position == selectedPosition).ToListAsync();

                foreach (var pv in positionVolunteers)
                {
                    pv.Position = null;
                }

                foreach (var shift in shiftsWithPosition)
                {
                    shift.Position = null;
                }

                selectedPosition.Deleted = true;
                await _context.SaveChangesAsync();
                return $"You successfully removed {selectedPosition.Name} from the list of positions.";
            }
            return $"Error: You must select a position.";
        }

        public async Task<IActionResult> OnPostAddShift()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isStaff = await _userManager.IsInRoleAsync(user, "staff");

            //var shift = await MapShiftData(SelectedShift, new Shift());

            //if (shift.Description == "vol")
            //{
            //    return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
            //}

            //if (shift.Description == "pos")
            //{
            //    return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
            //}

            //if (shift.StartTime < DateTime.Now.AddDays(-1))
            //{
            //    if (isStaff)
            //        return RedirectToPage(new { statusMessage = "Error: Only administrators are able to add shifts that are before today's date." });
            //}

            //await _context.Shifts.AddAsync(shift);

            //await _context.SaveChangesAsync();

            //// schedule email notification for shift
            //if (shift.Volunteer != null)
            //{
            //    AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
            //    _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift);
            //}

            //await _context.SaveChangesAsync();

            //string successMessage;

            //if (shift.Volunteer == null)
            //{
            //    successMessage = $"You successfully added an open shift for {shift.PositionWorked.Name} on " +
            //    $"{shift.StartTime.ToString("D")} from " +
            //    $"{shift.StartTime.ToString()} to {shift.EndTime.ToString()}.";
            //}
            //else
            //{
            //    successMessage = $"You successfully added a shift for {shift.PositionWorked.Name} on " +
            //    $"{shift.StartTime.ToString("D")} from " +
            //    $"{shift.StartTime.ToString()} to {shift.EndTime.ToString()} worked by {shift.Volunteer.FirstName} {shift.Volunteer.LastName}.";
            //}

            //return RedirectToPage(new { statusMessage = successMessage });
            return null;
        }

        public async Task<IActionResult> OnPostAddManyShifts()
        {
            //SelectedShiftPosition = _context.Positions.ToList()[0].Name;
            //var user = await _userManager.GetUserAsync(User);
            //bool isStaff = await _userManager.IsInRoleAsync(user, "staff");
            //bool isOpen = false;
            //string volstr = "";
            //string dateStr = "";
            //string startTimeStr = "";
            //string endTimeStr = "";
            //foreach (var position in ShiftAmounts)
            //{
            //    var shiftAmount = Convert.ToInt32(position.Value);
            //    if (shiftAmount > 0)
            //    {
            //        for (int i = 0; i < shiftAmount; i++)
            //        {
            //            var shift = await MapShiftData(SelectedShift, new Shift());
            //            isOpen = SelectedShift.Volunteer == null;
            //            dateStr = shift.StartTime.ToString("D");
            //            startTimeStr = shift.StartTime.ToString();
            //            endTimeStr = shift.EndTime.ToString();
            //            if (!isOpen)
            //            {
            //                volstr = shift.Volunteer.FirstName + " " + shift.Volunteer.LastName;
            //            }
            //            shift.PositionWorked = _context.Positions.FirstOrDefault(p => p.Id == position.Key);

            //            if (shift.Description == "vol")
            //            {
            //                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
            //            }

            //            if (shift.Description == "pos")
            //            {
            //                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
            //            }

            //            if (shift.StartTime < DateTime.Now.AddDays(-1))
            //            {
            //                if (isStaff)
            //                    return RedirectToPage(new { statusMessage = "Error: Only administrators are able to add shifts that are before today's date." });
            //            }

            //            await _context.Shifts.AddAsync(shift);
            //            await _context.SaveChangesAsync();

            //            if (shift.Volunteer != null)
            //            {
            //                AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
            //                _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift);
            //            }

            //            await _context.SaveChangesAsync();
            //        }
            //    }

            return null;
        }

        public class EditParams
        {
            public string key { get; set; }
            public string action { get; set; }
            public List<ShiftReadEditDto> added { get; set; }
            public List<ShiftReadEditDto> changed { get; set; }
            public List<ShiftReadEditDto> deleted { get; set; }
            public ShiftReadEditDto value { get; set; }
        }
    }
}


//public async Task<IActionResult> OnPostAddManyRecurringShifts()
//{
//    // so mapshift data doesnt return null
//    SelectedShiftPosition = _context.Positions.ToList()[0].Name;
//    bool isOpen = false;
//    string volstr = "";
//    string startDateStr = "";
//    string endDateStr = "";
//    string startTimeStr = "";
//    string endTimeStr = "";
//    string weekdayStr = "";
//    foreach (var position in ShiftAmounts)
//    {
//        int shiftAmount = Convert.ToInt32(position.Value);
//        if (shiftAmount > 0)
//        {
//            for (int i = 0; i < shiftAmount; i++)
//            {
//                if (SelectedShift.StartDate > SelectedShift.EndDate)
//                {
//                    return await OnGet("Error: The start date must be before the end date.");
//                }
//                var shift = await MapShiftData(SelectedShift, new RecurringShift());
//                isOpen = SelectedShift.Volunteer == null;
//                startDateStr = shift.StartTime.ToString("D");
//                endDateStr = shift.EndDate.ToString("D");
//                startTimeStr = shift.StartTime.ToString();
//                endTimeStr = shift.EndTime.ToString();
//                if (!isOpen)
//                {
//                    volstr = shift.Volunteer.FirstName + " " + shift.Volunteer.LastName;
//                }
//                weekdayStr = shift.NormalizedWeekdays;
//                shift.PositionWorked = _context.Positions.FirstOrDefault(p => p.Id == position.Key);

//                if (shift.Description == "vol")
//                {
//                    return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
//                }

//                if (shift.Description == "pos")
//                {
//                    return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
//                }

//                if (string.IsNullOrEmpty(shift.Weekdays))
//                {
//                    return RedirectToPage(new { statusMessage = "Error: You must select weekdays on which the shift should repeat." });
//                }

//                _context.RecurringShifts.Add(shift);

//                // the first call establishes the new shift in the db with an id so that the reminder will be created properly
//                await _context.SaveChangesAsync();

//                // schedule email notification for shift
//                if (shift.Volunteer != null)
//                {
//                    AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
//                    _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift);
//                }

//                await _context.SaveChangesAsync();
//            }
//        }
//    }

//    string successMessage;

//    if (isOpen)
//    {
//        successMessage = $"You successfully added multiple recurring sets of open shifts starting on " +
//        $"{startDateStr} and ending on {endDateStr} from " +
//        $"{startTimeStr} to {endTimeStr} occuring every {weekdayStr.TrimEnd(' ').TrimEnd(',')}";
//    }
//    else
//    {
//        successMessage = $"You successfully added multiple recurring sets of shifts starting on " +
//        $"{startDateStr} and ending on {endDateStr} from " +
//        $"{startTimeStr} to {endTimeStr} occuring every {weekdayStr.TrimEnd(' ').TrimEnd(',')} worked by {volstr}.";
//    }

//    return RedirectToPage(new { statusMessage = successMessage });
//}

//public async Task<IActionResult> OnPostAddRecurringShift()
//{
//    if (SelectedShift.StartDate > SelectedShift.EndDate)
//    {
//        return await OnGet("Error: The start date must be before the end date.");
//    }
//    var shift = (RecurringShift)(await MapShiftData(SelectedShift, new RecurringShift()));

//    if (shift.Description == "vol")
//    {
//        return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
//    }

//    if (shift.Description == "pos")
//    {
//        return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
//    }

//    if (string.IsNullOrEmpty(shift.Weekdays))
//    {
//        return RedirectToPage(new { statusMessage = "Error: You must select weekdays on which the shift should repeat." });
//    }

//    _context.RecurringShifts.Add(shift);

//    // the first call establishes the new shift in the db with an id so that the reminder will be created properly
//    await _context.SaveChangesAsync();

//    // schedule email notification for shift
//    if (shift.Volunteer != null)
//    {
//        AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
//        _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift);
//    }

//    await _context.SaveChangesAsync();

//    string successMessage;

//    if (shift.Volunteer == null)
//    {
//        successMessage = $"You successfully added a recurring set of open shifts for {shift.PositionWorked.Name} starting on " +
//        $"{shift.StartTime.ToString("D")} and ending on {shift.EndDate.ToString("D")} from " +
//        $"{shift.StartTime.ToString()} to {shift.EndTime.ToString()} occuring every {shift.NormalizedWeekdays}.";
//    }
//    else
//    {
//        successMessage = $"You successfully added a recurring set of shifts for {shift.PositionWorked.Name} starting on " +
//        $"{shift.StartTime.ToString("D")} and ending on {shift.EndDate.ToString("D")} from " +
//        $"{shift.StartTime.ToString()} to {shift.EndTime.ToString()} occuring every {shift.NormalizedWeekdays.TrimEnd(' ').TrimEnd(',')} worked by {shift.Volunteer.FirstName} {shift.Volunteer.LastName}.";
//    }

//    return RedirectToPage(new { statusMessage = successMessage });

//public async Task<IActionResult> OnPostEditShift()
//{
//    //var shift = await _context.Shifts.Include(x => x.Volunteer)
//    //    .Include(v => v.PositionWorked)
//    //    .FirstOrDefaultAsync(x => x.Id == SelectedShift.Id);

//    //var initialShift = _mapper.Map<Shift>(shift);

//    //_context.Shifts.Update(shift);

//    //// check if shift had a volunteer
//    //if (shift.Volunteer != null)
//    //{
//    //    // if so, cancel the notification scheduled for it
//    //    _reminderManager.CancelReminder(shift);
//    //}

//    //shift = await MapShiftData(SelectedShift, shift);

//    //if (shift.Description == "vol")
//    //{
//    //    return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
//    //}

//    //if (shift.Description == "pos")
//    //{
//    //    return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
//    //}

//    //await _context.SaveChangesAsync();

//    //// check if shift has volunteer after edit
//    //if (shift.Volunteer != null)
//    //{
//    //    // if so, schedule a reminder for it
//    //    AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
//    //    _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift);
//    //}

//    //await _context.SaveChangesAsync();

//    //return RedirectToPage(new { statusMessage = "You successfully edited the chosen shift." });
//}

//public async Task<IActionResult> OnPostDeleteShift()
//{
//    //Shift shift = _context.Shifts.Include(s => s.Volunteer).FirstOrDefault(x => x.Id == SelectedShift.Id);
//    //List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
//    //    .Include(p => p.RequestedShift)
//    //    .Include(p => p.OriginalShift)
//    //    .Where(a => (a.RequestedShift == shift || a.OriginalShift == shift) && a.Status == ShiftRequestAlert.RequestStatus.Pending)
//    //    .ToListAsync();

//    //if (shift != null)
//    //{
//    //    if (alertsWithChosenShift.Any())
//    //    {
//    //        StatusMessage =
//    //            $"Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted.          <form method=\"post\"><button class=\"btn btn-primary btn-sm\" asp-page-handler=\"ViewRequest\" asp-route-id\"{alertsWithChosenShift.FirstOrDefault().Id}\">See Request</button></form>";
//    //    }
//    //    else
//    //    {
//    //        // check if shift had a volunteer
//    //        if (shift.Volunteer != null)
//    //        {
//    //            // if so, cancel the notification scheduled for it
//    //            _reminderManager.CancelReminder(shift);
//    //        }

//    //        _context.Shifts.Remove(shift);
//    //        StatusMessage = "You succesfully deleted the chosen shift";
//    //        await _context.SaveChangesAsync();
//    //    }
//    //}

//    //return RedirectToPage(new { statusMessage = StatusMessage });
//}

//public async Task<IActionResult> OnPostEditRecurringShift()
//{
//    var recurringShift = _context.RecurringShifts
//        .Include(p => p.Volunteer)
//        .Include(p => p.PositionWorked)
//        .Include(p => p.ExcludedShifts)
//        .FirstOrDefault(x => x.Id == SelectedShift.Id);

//    if (RecurrenceSetStartDate > SelectedShift.EndDate)
//    {
//        return await OnGet("Error: The start date must be before the end date.");
//    }

//    _context.Update(recurringShift);

//    recurringShift = await EditRecurringShift(recurringShift);

//    if (recurringShift.Description == "vol")
//    {
//        return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
//    }

//    if (recurringShift.Description == "pos")
//    {
//        return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
//    }

//    await _context.SaveChangesAsync();

//    return RedirectToPage(new { statusMessage = "You have successfully edited the chosen shift." });
//}

//public async Task<IActionResult> OnPostDeleteShiftFromRecurringSet()
//{
//    // Only for shifts that have already been edited for their recurring set.
//    //TODO: Come back and decide what to do for alerts/reminders.
//    Shift shift = _context.Shifts.FirstOrDefault(x => x.Id == SelectedShift.Id);

//    List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
//        .Include(p => p.RequestedShift)
//        .Include(p => p.OriginalShift)
//        .Where(a => a.RequestedShift == shift || a.OriginalShift == shift)
//        .ToListAsync();

//    if (shift != null)
//    {
//        if (alertsWithChosenShift.Any())
//        {
//            return RedirectToPage(new { statusMessage = $"Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted." });
//        }
//        else
//        {
//            _context.Update(shift);

//            await _context.Entry(shift).Reference(p => p.Volunteer).LoadAsync();
//            // check if shift had a volunteer
//            if (shift.Volunteer != null)
//            {
//                // if so, cancel the notification scheduled for it
//                _reminderManager.CancelReminder(shift);
//            }

//            // shift is hidden from the calendar but will stay in the 
//            // excludedshifts list of it's parent recurring shift
//            shift.Hidden = true;
//            await _context.SaveChangesAsync();
//        }
//    }

//    return RedirectToPage(new { statusMessage = "You succesfully deleted the chosen shift from the recurring set." });
//}



//public async Task<IActionResult> OnPostDeleteRecurringShift()
//{
//    RecurringShift recurringShift = _context.RecurringShifts.Include(y => y.Volunteer).Include(v => v.ExcludedShifts).FirstOrDefault(x => x.Id == SelectedShift.Id);

//    List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
//        .Include(p => p.RequestedShift)
//        .Include(p => p.OriginalShift)
//        .Where(a => a.RequestedShift == recurringShift || a.OriginalShift == recurringShift)
//        .ToListAsync();

//    if (alertsWithChosenShift.Any())
//    {
//        return RedirectToPage(new { statusMessage = $"Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted." });
//    }

//    _context.Update(recurringShift);

//    recurringShift = DeleteRecurringShift(recurringShift);

//    await _context.SaveChangesAsync();

//    return RedirectToPage(new { statusMessage = "You succesfully deleted the chosen recurring set of shifts." });
//}

//public async Task<IActionResult> OnPostRestoreShiftPropertiesToOriginal()
//{
//    var shift = _context.Shifts
//        .Include(p => p.ParentRecurringShift).ThenInclude(p => p.ExcludedShifts)
//        .Include(p => p.ParentRecurringShift).ThenInclude(p => p.Volunteer)
//        .Include(p => p.ParentRecurringShift).ThenInclude(p => p.PositionWorked)
//        .FirstOrDefault(x => x.Id == SelectedShift.Id);

//    var alertsWithChosenShift = await _context.ShiftAlerts
//        .Include(p => p.RequestedShift)
//        .Include(p => p.OriginalShift)
//        .Where(a => a.RequestedShift == shift || a.OriginalShift == shift)
//        .ToListAsync();

//    if (shift != null)
//    {
//        if (alertsWithChosenShift.Any())
//        {
//            return RedirectToPage(new { statusMessage = $"Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted." });
//        }
//        else
//        {
//            await _context.Entry(shift).Reference(p => p.Volunteer).LoadAsync();
//            // check if shift had a volunteer
//            if (shift.Volunteer != null)
//            {
//                // if so, cancel the notification scheduled for it
//                var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.ShiftId == shift.Id);
//                _reminderManager.CancelReminder(shift, shift.StartTime);
//            }

//            _context.Update(shift.ParentRecurringShift);

//            // check to see if selected shift had its start date or time edited
//            var link = await _context.ShiftLinks
//                .Include(l => l.OriginalShift)
//                .FirstOrDefaultAsync(s => s.NewShift.Id == shift.Id);
//            bool startDateOrTimeWasEdited = link != null;

//            // if so, use the associated link to remove the shift created 
//            // in its place from the recurring set's excluded shifts.

//            if (startDateOrTimeWasEdited)
//            {
//                shift.ParentRecurringShift.ExcludedShifts.Remove(link.OriginalShift);
//                _context.Shifts.Remove(link.OriginalShift);
//                _context.ShiftLinks.Remove(link);
//            }

//            _context.Shifts.Remove(shift);
//            shift.ParentRecurringShift.ExcludedShifts.Remove(shift);
//            shift.ParentRecurringShift.UpdateRecurrenceRule();

//            // check if parent recurring shift has volunteer
//            if (shift.ParentRecurringShift.Volunteer != null)
//            {
//                var volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.ParentRecurringShift.Volunteer.Id);

//                // cancel old reminders for recurring shift
//                _reminderManager.CancelReminder(shift.ParentRecurringShift);
//                // schedule new ones
//                _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.ParentRecurringShift.Volunteer, shift.ParentRecurringShift);
//            }

//            await _context.SaveChangesAsync();
//        }
//    }

//    return RedirectToPage(new { statusMessage = "You have successfully returned the chosen shift's properties to their original state." });
//}



//private async Task<Shift> MapShiftData(ShiftReadEditDto dto, Shift shift)
//{
//    VolunteerProfile volunteer = null;
//    if (SelectedShiftVolunteer != null)
//    {
//        if (int.TryParse(SelectedShiftVolunteer.Split(' ')[0], out int volunteerId))
//        {
//            volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(x => x.Id == volunteerId);
//        }
//        else
//        {
//            if (shift is RecurringShift)
//            {
//                return new RecurringShift { Description = "vol" };
//            }
//            else
//            {
//                return new Shift { Description = "vol" };
//            }
//        }

//    }

//    Position pos = await _context.Positions.FirstOrDefaultAsync(x => x.Name == SelectedShiftPosition);

//    if (pos == null)
//    {
//        if (shift is RecurringShift)
//        {
//            return new RecurringShift { Description = "pos" };
//        }
//        else
//        {
//            return new Shift { Description = "pos" };
//        }
//    }

//    if (shift is RecurringShift recurringShift)
//    {
//        _mapper.Map(dto, recurringShift);
//        recurringShift.Weekdays = GetSelectedWeekdays();
//        recurringShift.UpdateRecurrenceRule();
//    }
//    else
//    {
//        _mapper.Map(dto, shift);
//    }

//    shift.Volunteer = volunteer;
//    shift.PositionWorked = pos;
//    shift.CreateDescription();

//    return shift;
//}

//// convert the boolean weekday properties into the format required by rrule
//public string GetSelectedWeekdays()
//{
//    string selectedDays = "";

//    if (Sunday)
//    {
//        selectedDays += "SU,";
//    }
//    if (Monday)
//    {
//        selectedDays += "MO,";
//    }
//    if (Tuesday)
//    {
//        selectedDays += "TU,";
//    }
//    if (Wednesday)
//    {
//        selectedDays += "WE,";
//    }
//    if (Thursday)
//    {
//        selectedDays += "TH,";
//    }
//    if (Friday)
//    {
//        selectedDays += "FR,";
//    }
//    if (Saturday)
//    {
//        selectedDays += "SA,";
//    }

//    selectedDays = selectedDays.TrimEnd(',');

//    return selectedDays;
//}



//private async Task<JsonResult> EditRecurringShift(Shift recShift)
//{

//    //if (EditType == RecurringShiftEditType.Single)
//    //{
//    //    recShift = await EditSingleShiftFromRecurringSet(recShift);
//    //}
//    //else if (EditType == RecurringShiftEditType.WholeSet)
//    //{
//    //    recShift = await EditWholeRecurringSet(recShift);
//    //}

//    //recShift.UpdateRecurrenceRule();

//    //return recShift;
//    return null
//}

//private JsonResult DeleteRecurringShift(RecurringShift recShift)
//{
//    //if (EditType == RecurringShiftEditType.Single)
//    //{
//    //    DeleteSingleShiftFromRecurringSet(recShift);
//    //}
//    //else if (EditType == RecurringShiftEditType.WholeSet)
//    //{
//    //    DeleteWholeRecurringSet(recShift);
//    //}

//    //return recShift;
//}

//private async Task<RecurringShift> EditSingleShiftFromRecurringSet(RecurringShift recShift)
//{
//    // check if shift had a volunteer
//    if (recShift.Volunteer != null)
//    {
//        // if so, cancel the original notification scheduled for it
//        _reminderManager.CancelReminder(recShift, OriginalStartDate);
//    }

//    // make a new shift which will be excluded from the selected recurring shift, 
//    // map the the selected recurring shift's properties to it, 
//    // load the recurring shift's excluded shifts 
//    // and add the new shift to the list of excluded shifts
//    var excludedShift = new Shift();

//    excludedShift = await MapShiftData(SelectedShift, excludedShift);
//    // set the excluded shift's id to 0 and it's parent recurring shift to the currently selected
//    // recurring shift

//    if (excludedShift.Description == "vol")
//    {
//        return new RecurringShift() { Description = "vol" };
//    }

//    if (excludedShift.Description == "pos")
//    {
//        return new RecurringShift() { Description = "pos" };
//    }

//    excludedShift.Id = 0;
//    excludedShift.ParentRecurringShift = _context.RecurringShifts
//        .FirstOrDefault(s => s.Id == SelectedShift.Id);

//    var newStartTime = excludedShift.StartTime;
//    var newStartDate = excludedShift.StartTime;

//    // handle all special cases
//    if (recShift.StartTime != newStartTime)
//    {
//        // if start time has been changed, an additional shift entity with the original start time 
//        // must be created and added to the recurring shift's excluded shifts so that the shift isn't duplicated
//        var excludedShiftWithOriginalStartTime = new Shift
//        {
//            StartTime = recShift.StartTime,
//            StartTime = OriginalStartDate,
//            Hidden = true
//        };

//        recShift.ExcludedShifts.Add(excludedShiftWithOriginalStartTime);

//        // a link between the recurring shift, the new shift and the excluded shift must be created here
//        var link = new RecurringChildLink
//        {
//            ParentSet = recShift,
//            OriginalShift = excludedShiftWithOriginalStartTime,
//            NewShift = excludedShift
//        };

//        _context.Add(link);
//        _context.Add(excludedShift);
//    }
//    else if (OriginalStartDate != newStartDate)
//    {
//        // if start date has been changed, an additional shift entity with the original start date
//        // must be created and added to the recurring shift's excluded shifts so that the shift isn't duplicated
//        // (same process as changed start time handling above)
//        Shift excludedShiftWithOriginalStartDate = new Shift
//        {
//            StartTime = recShift.StartTime,
//            StartTime = OriginalStartDate,
//            Hidden = true
//        };

//        recShift.ExcludedShifts.Add(excludedShiftWithOriginalStartDate);

//        // a link between the recurring shift, the new shift and the excluded shift must be created here
//        var link = new RecurringChildLink
//        {
//            ParentSet = recShift,
//            OriginalShift = excludedShiftWithOriginalStartDate,
//            NewShift = excludedShift
//        };

//        _context.Add(link);
//        _context.Add(excludedShift);
//    }
//    else
//    // trivial case: something besides start date or start time has been changed
//    {
//        recShift.ExcludedShifts.Add(excludedShift);
//    }

//    // delete the recurring shift if all of its child shifts are now excluded
//    bool allShiftsInRecurringSetAreExcluded = recShift.ConstituentShifts.Count == recShift.ExcludedShifts.Count;
//    if (allShiftsInRecurringSetAreExcluded)
//    {
//        _context.Remove(recShift);
//    }

//    recShift.UpdateRecurrenceRule();

//    await _context.SaveChangesAsync();

//    // check if recurring shift has a volunteer after being edited
//    if (excludedShift.Volunteer != null)
//    {
//        // if so, schedule a new reminder for it
//        AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == excludedShift.Volunteer.Id);
//        _reminderManager.ScheduleReminder(volunteerAccount.Email, excludedShift.Volunteer, excludedShift);
//    }

//    return recShift;
//}

//private async Task<RecurringShift> EditWholeRecurringSet(RecurringShift recShift)
//{
//    // check if recurring shift had volunteer
//    if (recShift.Volunteer != null)
//    {
//        // if so, cancel the notification scheduled for it
//        _reminderManager.CancelReminder(recShift);
//    }

//    var initialStartTime = recShift.StartTime;

//    recShift = (RecurringShift)(await MapShiftData(SelectedShift, recShift));
//    // start date must be reassigned to the start date for the whole set
//    recShift.StartTime = RecurrenceSetStartDate;

//    var finalStartTime = recShift.StartTime;

//    if (!Equals(initialStartTime, finalStartTime))
//    {
//        foreach (var shift in recShift.ExcludedShifts)
//        {
//            shift.StartTime = recShift.StartTime;
//        }

//        recShift.UpdateRecurrenceRule();
//    }

//    await _context.SaveChangesAsync();

//    // check if recurring shift has volunteer after edit
//    if (recShift.Volunteer != null)
//    {
//        // if so, schedule a reminder for it
//        AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == recShift.Volunteer.Id);
//        _reminderManager.ScheduleReminder(volunteerAccount.Email, recShift.Volunteer, recShift);
//    }

//    return recShift;
//}

//private void DeleteSingleShiftFromRecurringSet(RecurringShift recShift)
//{
//    var shift = new Shift();
//    shift.StartTime = OriginalStartDate;
//    shift.StartTime = recShift.StartTime;
//    shift.Hidden = true;

//    recShift.ExcludedShifts.Add(shift);
//    recShift.UpdateRecurrenceRule();
//}

//private void DeleteWholeRecurringSet(RecurringShift recurringShift)
//{
//    // find all shifts in this recurring set that have already been deleted and clear then from the database
//    // all child shifts must be deleted or detached from the parent shift to avoid violating the foreign key constraint
//    foreach (var shift in recurringShift.ExcludedShifts)
//    {
//        bool shiftHasBeenDeleted = shift.Hidden;
//        if (shiftHasBeenDeleted)
//        {
//            _context.Remove(shift);
//        }
//        else
//        {
//            // if it hasn't been deleted from the set. let it remain and set its parentrecurringshiftproperty to null
//            _context.Update(shift);
//            shift.ParentRecurringShift = null;
//        }
//    }

//    recurringShift.ExcludedShifts.Clear();
//    _context.Shifts.Remove(recurringShift);

//    // check if shift had a volunteer
//    if (recurringShift.Volunteer != null)
//    {
//        // if so, cancel the notification scheduled for it
//        _reminderManager.CancelReminder(recurringShift);
//    }
//}
