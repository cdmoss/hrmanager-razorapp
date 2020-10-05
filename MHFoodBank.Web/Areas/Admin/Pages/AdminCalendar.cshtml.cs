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
using MHFoodBank.Common.Services;

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

        private void ScheduleReminder(Shift newShift)
        {
            //schedule email notification for shift if shift is after todays date if it's not an open shift
            if (newShift.VolunteerProfileId != null)
            {
                // if shift is recurring, check each child shift to see if the recurrence period requires reminders
                if (newShift.IsRecurrence)
                {
                    var recurrenceDates = RecurrenceHelper.GetRecurrenceDateTimeCollection(newShift.RecurrenceRule, newShift.StartTime);
                    if (recurrenceDates.Any(d => d > DateTime.Now.AddDays(1).Date))
                    {
                        var volunteerAccount = _context.Users.FirstOrDefault(u => u.VolunteerProfile.Id == newShift.VolunteerProfileId);
                        _reminderManager.ScheduleReminder(volunteerAccount, newShift);
                    }
                }
                // if it isn't recurring then simply schedule a reminder
                else if (newShift.StartTime > DateTime.Now.AddDays(1).Date)
                {
                    var volunteerAccount = _context.Users.FirstOrDefault(u => u.VolunteerProfile.Id == newShift.VolunteerProfileId);
                    _reminderManager.ScheduleReminder(volunteerAccount, newShift);
                }
            }
        }

        private void InsertShift(CRUDModel<ShiftReadEditDto> shiftDto)
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
            newShift.IsRecurrence = !string.IsNullOrWhiteSpace(newShift.RecurrenceRule);

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
                        var chosenVolunteer = newShift.VolunteerProfileId != null ? _context.VolunteerProfiles
                            .Where(p => p.Id == newShift.VolunteerProfileId).FirstOrDefault() : null;

                        newShift.Subject = newShift.VolunteerProfileId == null ?
                            $"Open - {chosenPosition.Name}" :
                            $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

                        _context.Shifts.Add(newShift);

                        _context.SaveChanges();

                        ScheduleReminder(newShift);
                    }
                }
            }
            else
            {
                var chosenPosition = _context.Positions.FirstOrDefault(p => p.Id == newShift.PositionId);
                var chosenVolunteer = newShift.VolunteerProfileId != null ? _context.VolunteerProfiles
                    .Where(p => p.Id == newShift.VolunteerProfileId).FirstOrDefault() : null;

                newShift.Subject = newShift.VolunteerProfileId == null ?
                    $"Open - {chosenPosition.Name}" :
                    $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

                _context.Shifts.Add(newShift);

                _context.SaveChanges();

                ScheduleReminder(newShift);
            }
        }

        // this method cancels the reminder for the shift being removed as a side effect
        private bool CheckIfShiftIsBeingRemovedFromRecurringSet(ShiftReadEditDto newShiftDto, Shift shiftBeingUpdated)
        {
            // determine if the shift selected for change is a shift that's being removed from its recurring set
            var oldExDates = shiftBeingUpdated.RecurrenceException == null ? new string[0] : shiftBeingUpdated.RecurrenceException.Split(',');
            var newExDates = newShiftDto.RecurrenceException == null ? new string[0] : newShiftDto.RecurrenceException.Split(',');
            bool shiftIsBeingRemovedFromRecurringSet = newExDates.Length > oldExDates.Length;

            // if it is, pass in the date of the last shift excluded when cancelling reminder
            if (shiftIsBeingRemovedFromRecurringSet)
            {
                var newExDate = RecurrenceHelper.ConvertExDateStringToDateTime(newExDates[newExDates.Length - 1]);
                _reminderManager.CancelReminder(shiftBeingUpdated, newExDate);

                shiftBeingUpdated.RecurrenceException = newShiftDto.RecurrenceException;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateShiftProperties(ShiftReadEditDto newShiftDto, Shift shiftBeingUpdated)
        {
            shiftBeingUpdated.VolunteerProfileId = newShiftDto.VolunteerProfileId;
            shiftBeingUpdated.StartTime = newShiftDto.StartTime.AddHours(-6);
            shiftBeingUpdated.EndTime = newShiftDto.EndTime.AddHours(-6);
            shiftBeingUpdated.PositionId = newShiftDto.PositionId;
            shiftBeingUpdated.VolunteerProfileId = newShiftDto.VolunteerProfileId;
            shiftBeingUpdated.IsAllDay = newShiftDto.IsAllDay;
            shiftBeingUpdated.RecurrenceRule = newShiftDto.RecurrenceRule;
            shiftBeingUpdated.RecurrenceID = newShiftDto.RecurrenceID;
            shiftBeingUpdated.RecurrenceException = newShiftDto.RecurrenceException;
            shiftBeingUpdated.IsRecurrence = !string.IsNullOrWhiteSpace(shiftBeingUpdated.RecurrenceRule);
            if (shiftBeingUpdated.VolunteerProfileId == 0)
            {
                shiftBeingUpdated.VolunteerProfileId = null;
            }
        }

        public void UpdateShift(CRUDModel<ShiftReadEditDto> shiftDto)
        {
            var newShiftDto = (shiftDto.Action == "update") ? shiftDto.Value : shiftDto.Changed[0];
            var shiftBeingUpdated = _context.Shifts.FirstOrDefault(c => c.Id == Convert.ToInt32(newShiftDto.Id));

            bool shiftIsBeingRemovedFromRecurringSet = CheckIfShiftIsBeingRemovedFromRecurringSet(newShiftDto, shiftBeingUpdated);

            if (shiftBeingUpdated != null && !shiftIsBeingRemovedFromRecurringSet)
            {
                // cancel old reminder if old version of shift had a volunteer
                if (shiftBeingUpdated.VolunteerProfileId != null)
                {
                    _reminderManager.CancelReminder(shiftBeingUpdated);
                }

                UpdateShiftProperties(newShiftDto, shiftBeingUpdated);

                var chosenPosition = _context.Positions.FirstOrDefault(p => p.Id == shiftBeingUpdated.PositionId);
                var chosenVolunteer = shiftBeingUpdated.VolunteerProfileId != null ? _context.VolunteerProfiles
                    .FirstOrDefault(p => p.Id == shiftBeingUpdated.VolunteerProfileId) : null;

                shiftBeingUpdated.Subject = shiftBeingUpdated.VolunteerProfileId == null ?
                $"Open - {chosenPosition.Name}" :
                    $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

                _context.SaveChanges();

                //create new reminder if new version of shift has volunteer and if this isn't a request to delete a child shift from its recurring set
                if (shiftBeingUpdated.VolunteerProfileId != null)
                {
                    _context.Entry(chosenVolunteer).Reference(v => v.User).Load();
                    _reminderManager.ScheduleReminder(chosenVolunteer.User, shiftBeingUpdated);
                }
            }
        }

        private void RemoveShift(CRUDModel<ShiftReadEditDto> shiftDto)
        {
            if (shiftDto.Action == "remove")
            {
                int key = Convert.ToInt32(shiftDto.Key);
                var newShift = _context.Shifts.Where(c => c.Id == key).FirstOrDefault();
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
                    var newShift = _context.Shifts.Where(c => c.Id == shifts.Id).FirstOrDefault();
                    if (newShift != null)
                    {
                        _reminderManager.CancelReminder(newShift);
                        _context.Shifts.Remove(newShift);
                    }
                }
            }
            _context.SaveChanges();
        }

        public JsonResult OnPostUpdateShifts([FromBody] CRUDModel<ShiftReadEditDto> shiftDto)
        {
            if (shiftDto != null)
            {
                if (shiftDto.Action == "insert" || (shiftDto.Action == "batch" && shiftDto.Added.Count > 0)) // this block of code will execute while inserting the appointments
                {
                    InsertShift(shiftDto);
                }
                // when a shift is deleted from a recurring shift it enters this block
                if (shiftDto.Action == "update" || (shiftDto.Action == "batch" && shiftDto.Changed.Count > 0)) // this block of code will execute while updating the appointment
                {
                    UpdateShift(shiftDto);
                }
                if (shiftDto.Action == "remove" || (shiftDto.Action == "batch" && shiftDto.Deleted.Count > 0)) // this block of code will execute while removing the appointment
                {
                    RemoveShift(shiftDto);
                };
            }

            var shiftDomainModels = _context.Shifts.ToList();
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
    }
}