using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Hangfire.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using MHFoodBank.Common.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using MHFoodBank.Web.Services;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Identity;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin" )]
    public class AdminCalendar : AdminPageModel
    {
        private readonly IReminderManager _reminderManager;
        private readonly UserManager<AppUser> _userManager;
        public enum RecurringShiftEditType
        {
            Single,
            WholeSet
        }

        #region model properties
        // for choosing a volunteer when editing/adding a shift
        [BindProperty]
        public List<Position> Positions { get; set; }
        [BindProperty]
        public string SelectedShiftPosition { get; set; }
        [BindProperty]
        public string SelectedShiftVolunteer{ get; set; }
        [BindProperty]
        public DateTime OriginalStartDate { get; set; }
        // Created this property to grab the original start date for the whole recurring set.
        // This is needed along side selectedshift.startdate in the edit rec shift modal for the changing between single shift
        // and whole recurring set start date.
        [BindProperty]
        public DateTime RecurrenceSetStartDate { get; set; }
        [BindProperty]
        public ShiftReadEditDto SelectedShift { get; set; }
        // for choosing a volunteer when editing/adding a shift
        [BindProperty]
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        // the position displayed on page load
        [BindProperty(SupportsGet = true)]
        public Position DefaultPosition { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }
        [BindProperty]
        public Position SearchedPosition { get; set; }
        [BindProperty]
        // position that was selected in the edit/delete position window
        public Position SelectedPosition { get; set; }
        [BindProperty]
        public string NewPositionName { get; set; }
        // give user feedback after action
        public string StatusMessage { get; set; }

        // to change behaviour of recurring shift edit process, 
        // switches between editing only the selected shift or the whole recurring set
        [BindProperty]
        public RecurringShiftEditType EditType { get; set; }
        #region weekdays for recurring shift add/edit window
        // these are bound to the checkboxes on the recurring shift add/edit window
        [BindProperty]
        public bool Sunday { get; set; }
        [BindProperty]
        public bool Monday { get; set; }
        [BindProperty]
        public bool Tuesday { get; set; }
        [BindProperty]
        public bool Wednesday { get; set; }
        [BindProperty]
        public bool Thursday { get; set; }
        [BindProperty]
        public bool Friday { get; set; }
        [BindProperty]
        public bool Saturday { get; set; }
        #endregion

        // populates the calendar with shifts currently in the db
        // Testing to see if we can just use recurring shift read edit dtos
        public List<ShiftReadEditDto> Shifts { get; set; }
        #endregion

        private readonly IMapper _mapper;

        public AdminCalendar(FoodBankContext context, UserManager<AppUser> userManager, IMapper mapper, IReminderManager reminderManager, string currentPage = "Scheduling") : base(context, currentPage)
        {
            _mapper = mapper;
            _reminderManager = reminderManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet(string statusMessage = null)
        {
            await PrepareModel(statusMessage);

            return Page();
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

            Position position = new Position { Name = NewPositionName };
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You successfully added {position.Name} to the list of positions." });
        }

        public async Task<IActionResult> OnPostEditPosition()
        {
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

            SelectedPosition = _context.Positions.FirstOrDefault(p => p.Id == Convert.ToInt32(SelectedShiftPosition[0]));

            string resultStatus = await EditPosition(SelectedPosition);
            return RedirectToPage(new { statusMessage = resultStatus });
        }

        public async Task<IActionResult> OnPostRemovePosition()
        {
            var selectedPos = await _context.Positions.FirstOrDefaultAsync(p => p.Id == Convert.ToInt32(SelectedShiftPosition[0]));
            SelectedPosition = _context.Positions.FirstOrDefault(p => p.Name == selectedPos.Name);
            string resultStatus = await RemovePosition(SelectedPosition);
            return RedirectToPage(new { statusMessage = resultStatus });
        }

        public async Task<IActionResult> OnPostAddShift()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isStaff = await _userManager.IsInRoleAsync(user, "staff");

            var shift = await MapShiftData(SelectedShift, new Shift());

            if (shift.Description == "vol")
            {
                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
            }

            if (shift.Description == "pos")
            {
                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
            }

            if (shift.StartDate < DateTime.Now.AddDays(-1))
            {
                if(isStaff)
                    return RedirectToPage(new { statusMessage = "Error: Only administrators are able to add shifts that are before today's date." });
            }

            await _context.Shifts.AddAsync(shift);

            await _context.SaveChangesAsync();

            // schedule email notification for shift
            if (shift.Volunteer != null)
            {
                AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
                _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift);
            }

            await _context.SaveChangesAsync();

            string successMessage;

            if (shift.Volunteer == null)
            {
                successMessage = $"You successfully added an open shift for {shift.PositionWorked.Name} on " +
                $"{shift.StartDate.ToString("D")} from " +
                $"{shift.StartTime.ToString()} to {shift.EndTime.ToString()}.";
            }
            else
            {
                successMessage = $"You successfully added a shift for {shift.PositionWorked.Name} on " +
                $"{shift.StartDate.ToString("D")} from " +
                $"{shift.StartTime.ToString()} to {shift.EndTime.ToString()} worked by {shift.Volunteer.FirstName} {shift.Volunteer.LastName}.";
            }

            return RedirectToPage(new { statusMessage = successMessage });
        }


        public async Task<IActionResult> OnPostAddRecurringShift()
        {
            if(SelectedShift.StartDate > SelectedShift.EndDate)
            {
                return await OnGet("Error: The start date must be before the end date.");
            }
            var shift = (RecurringShift)(await MapShiftData(SelectedShift, new RecurringShift()));

            if (shift.Description == "vol")
            {
                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
            }

            if (shift.Description == "pos")
            {
                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
            }

            if (string.IsNullOrEmpty(shift.Weekdays))
            {
                return RedirectToPage(new { statusMessage = "Error: You must select weekdays on which the shift should repeat."});
            }

            _context.RecurringShifts.Add(shift);

            // the first call establishes the new shift in the db with an id so that the reminder will be created properly
            await _context.SaveChangesAsync();

            // schedule email notification for shift
            if (shift.Volunteer != null)
            {
                AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
                _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift);
            }

            await _context.SaveChangesAsync();

            string successMessage;

            if (shift.Volunteer == null)
            {
                successMessage = $"You successfully added a recurring set of open shifts for {shift.PositionWorked.Name} starting on " +
                $"{shift.StartDate.ToString("D")} and ending on {shift.EndDate.ToString("D")} from " +
                $"{shift.StartTime.ToString()} to {shift.EndTime.ToString()} occuring every {shift.NormalizedWeekdays}.";
            }
            else
            {
                successMessage = $"You successfully added a recurring set of shifts for {shift.PositionWorked.Name} starting on " +
                $"{shift.StartDate.ToString("D")} and ending on {shift.EndDate.ToString("D")} from " +
                $"{shift.StartTime.ToString()} to {shift.EndTime.ToString()} occuring every {shift.NormalizedWeekdays.TrimEnd(' ').TrimEnd(',')} worked by {shift.Volunteer.FirstName} {shift.Volunteer.LastName}.";
            }

            return RedirectToPage(new { statusMessage = successMessage });
        }

        public async Task<IActionResult> OnPostEditShift()
        {
            var shift = await _context.Shifts.Include(x => x.Volunteer)
                .Include(v => v.PositionWorked)
                .FirstOrDefaultAsync(x => x.Id == SelectedShift.Id);

            var initialShift = _mapper.Map<Shift>(shift);

            _context.Shifts.Update(shift);

            // check if shift had a volunteer
            if (shift.Volunteer != null)
            {
                // if so, cancel the notification scheduled for it
                _reminderManager.CancelReminder(shift);
            }

            shift = await MapShiftData(SelectedShift, shift);

            if (shift.Description == "vol")
            {
                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
            }

            if (shift.Description == "pos")
            {
                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
            }

            await _context.SaveChangesAsync();

            // check if shift has volunteer after edit
            if (shift.Volunteer != null)
            {
                // if so, schedule a reminder for it
                AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
                _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = "You successfully edited the chosen shift." });
        }

        public async Task<IActionResult> OnPostEditRecurringShift()
        {
            var recurringShift = _context.RecurringShifts
                .Include(p => p.Volunteer)
                .Include(p => p.PositionWorked)
                .Include(p => p.ExcludedShifts)
                .FirstOrDefault(x => x.Id == SelectedShift.Id);

            if (RecurrenceSetStartDate > SelectedShift.EndDate)
            {
                return await OnGet("Error: The start date must be before the end date.");
            }

            _context.Update(recurringShift);

            recurringShift = await EditRecurringShift(recurringShift);

            if (recurringShift.Description == "vol")
            {
                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid volunteer ID or select a volunteer from the list." });
            }

            if (recurringShift.Description == "pos")
            {
                return RedirectToPage(new { statusMessage = "Error: Please either enter a valid position name or select a position from the list" });
            }

            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = "You have successfully edited the chosen shift." });
        }

        public async Task<IActionResult> OnPostDeleteShift()
        {
            Shift shift = _context.Shifts.Include(s => s.Volunteer).FirstOrDefault(x => x.Id == SelectedShift.Id);
            List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
                .Include(p => p.RequestedShift)
                .Include(p => p.OriginalShift)
                .Where(a => a.RequestedShift == shift || a.OriginalShift == shift)
                .ToListAsync();

            if (shift != null)
            {
                if (alertsWithChosenShift.Any())
                {
                    StatusMessage =
                        "Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted.";
                }
                else
                {
                    // check if shift had a volunteer
                    if (shift.Volunteer != null)
                    {
                        // if so, cancel the notification scheduled for it
                        _reminderManager.CancelReminder(shift);
                    }

                    _context.Shifts.Remove(shift);
                    StatusMessage = "You succesfully deleted the chosen shift";
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage(new { statusMessage = StatusMessage });
        }

        public async Task<IActionResult> OnPostDeleteShiftFromRecurringSet()
        {
            // Only for shifts that have already been edited for their recurring set.
            //TODO: Come back and decide what to do for alerts/reminders.
            Shift shift = _context.Shifts.FirstOrDefault(x => x.Id == SelectedShift.Id);

            List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
                .Include(p => p.RequestedShift)
                .Include(p => p.OriginalShift)
                .Where(a => a.RequestedShift == shift || a.OriginalShift == shift)
                .ToListAsync();

            if (shift != null)
            {
                if (alertsWithChosenShift.Any())
                {
                    return RedirectToPage(new { statusMessage = $"Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted." });
                }
                else
                {
                    _context.Update(shift);

                    await _context.Entry(shift).Reference(p => p.Volunteer).LoadAsync();
                    // check if shift had a volunteer
                    if (shift.Volunteer != null)
                    {
                        // if so, cancel the notification scheduled for it
                        _reminderManager.CancelReminder(shift);
                    }

                    // shift is hidden from the calendar but will stay in the 
                    // excludedshifts list of it's parent recurring shift
                    shift.Hidden = true;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage( new { statusMessage = "You succesfully deleted the chosen shift from the recurring set."});
        }



        public async Task<IActionResult> OnPostDeleteRecurringShift()
        {
            RecurringShift recurringShift = _context.RecurringShifts.Include(y => y.Volunteer).Include(v => v.ExcludedShifts).FirstOrDefault(x => x.Id == SelectedShift.Id);
            
            List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
                .Include(p => p.RequestedShift)
                .Include(p => p.OriginalShift)
                .Where(a => a.RequestedShift == recurringShift || a.OriginalShift == recurringShift)
                .ToListAsync();
            
            if (alertsWithChosenShift.Any())
            {
                return RedirectToPage(new { statusMessage = $"Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted." });
            }
            
            _context.Update(recurringShift);

            recurringShift = DeleteRecurringShift(recurringShift);

            await _context.SaveChangesAsync();

            return RedirectToPage( new { statusMessage = "You succesfully deleted the chosen recurring set of shifts."});
        }

        public async Task<IActionResult> OnPostRestoreShiftPropertiesToOriginal()
        {
            var shift = _context.Shifts
                .Include(p => p.ParentRecurringShift).ThenInclude(p => p.ExcludedShifts)
                .Include(p => p.ParentRecurringShift).ThenInclude(p => p.Volunteer)
                .Include(p => p.ParentRecurringShift).ThenInclude(p => p.PositionWorked)
                .FirstOrDefault(x => x.Id == SelectedShift.Id);

            var alertsWithChosenShift = await _context.ShiftAlerts
                .Include(p => p.RequestedShift)
                .Include(p => p.OriginalShift)
                .Where(a => a.RequestedShift == shift || a.OriginalShift == shift)
                .ToListAsync();

            if (shift != null)
            {
                if (alertsWithChosenShift.Any())
                {
                    return RedirectToPage(new { statusMessage = $"Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted." });
                }
                else
                {
                    await _context.Entry(shift).Reference(p => p.Volunteer).LoadAsync();
                    // check if shift had a volunteer
                    if (shift.Volunteer != null)
                    {
                        // if so, cancel the notification scheduled for it
                        var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.ShiftId == shift.Id);
                        _reminderManager.CancelReminder(shift, shift.StartDate);
                    }

                    _context.Update(shift.ParentRecurringShift);

                    // check to see if selected shift had its start date or time edited
                    var link = await _context.ShiftLinks
                        .Include(l => l.OriginalShift)
                        .FirstOrDefaultAsync(s => s.NewShift.Id == shift.Id);
                    bool startDateOrTimeWasEdited = link != null;

                    // if so, use the associated link to remove the shift created 
                    // in its place from the recurring set's excluded shifts.

                    if (startDateOrTimeWasEdited)
                    {
                        shift.ParentRecurringShift.ExcludedShifts.Remove(link.OriginalShift);
                        _context.Shifts.Remove(link.OriginalShift);
                        _context.ShiftLinks.Remove(link);
                    }

                    _context.Shifts.Remove(shift);
                    shift.ParentRecurringShift.ExcludedShifts.Remove(shift);
                    shift.ParentRecurringShift.UpdateRecurrenceRule();

                    // check if parent recurring shift has volunteer
                    if (shift.ParentRecurringShift.Volunteer != null)
                    {
                        var volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.ParentRecurringShift.Volunteer.Id);

                        // cancel old reminders for recurring shift
                        _reminderManager.CancelReminder(shift.ParentRecurringShift);
                        // schedule new ones
                        _reminderManager.ScheduleReminder(volunteerAccount.Email, shift.ParentRecurringShift.Volunteer, shift.ParentRecurringShift);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage(new { statusMessage = "You have successfully returned the chosen shift's properties to their original state."});
        }

        public async Task OnPostSearch()
        {
            await PrepareModel(null);

            var searcher = new Searcher(_context);
            SearchedPosition = searcher.GetSearchedPosition(Request);
            Shifts = searcher.FilterShiftsBySearch(Shifts, SearchedName, SearchedPosition);
        }

        //public async Task<IActionResult> OnPostSendNotifications()
        //{
        //    await PrepareModel(null);
        //
        //    return await SendNotifications();
        //}

        private async Task<Shift> MapShiftData(ShiftReadEditDto dto, Shift shift)
        {
            VolunteerProfile volunteer = null;
            if (SelectedShiftVolunteer != null)
            {
                if (int.TryParse(SelectedShiftVolunteer.Split(' ')[0], out int volunteerId))
                {
                    volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(x => x.Id == volunteerId);
                }
                else
                {
                    if (shift is RecurringShift)
                    {
                        return new RecurringShift { Description = "vol" };
                    }
                    else
                    {
                        return new Shift { Description = "vol" };
                    }
                }
                
            }

            Position pos = await _context.Positions.FirstOrDefaultAsync(x => x.Name == SelectedShiftPosition);

            if (pos == null)
            {
                if (shift is RecurringShift)
                {
                    return new RecurringShift { Description = "pos" };
                }
                else
                {
                    return new Shift { Description = "pos" };
                }
            }

            if (shift is RecurringShift recurringShift)
            {
                _mapper.Map(dto, recurringShift);
                recurringShift.Weekdays = GetSelectedWeekdays();
                recurringShift.UpdateRecurrenceRule();
            }
            else
            {
                _mapper.Map(dto, shift);
            }

            shift.Volunteer = volunteer;
            shift.PositionWorked = pos;
            shift.CreateDescription();

            return shift;
        }

        //private async Task<IActionResult> SendNotifications()
        //{
        //    // if there are no shifts in db, redirect immediately
        //    if (!_context.Shifts.Any())
        //    {
        //        return RedirectToPage();
        //    }
        //
        //    //// set up smtp client
        //    //SmtpClient client = new SmtpClient();
        //    //await client.ConnectAsync("smtp.gmail.com", 587);
        //
        //    // get all volunteers
        //    List<AppUser> volunteersUserAccounts = await _context.Users.Where(u => u.VolunteerProfile != null).ToListAsync();
        //
        //    // for each volunteer, prepare a list of shifts that agrees with their availability
        //    foreach (var volunteer in volunteersUserAccounts)
        //    {
        //        // if this volunteer has no availabilities, skip them
        //        if (!volunteer.VolunteerProfile.Availabilities.Any())
        //        {
        //            continue;
        //        }
        //
        //        await _context.Entry(volunteer.VolunteerProfile).Collection(p => p.Availabilities).LoadAsync();
        //        List<Shift> workableShifts = GetWorkableShiftsFromAvailabilites(volunteer.VolunteerProfile.Availabilities);
        //        bool noWorkableShifts = !workableShifts.Any();
        //
        //        // skip the volunteer if they can't work any shifts
        //        if (noWorkableShifts)
        //        {
        //            continue;
        //        }
        //
        //        // send them an email notification if there are shifts they can work
        //        MimeMessage email = CreateEmailMessage(volunteer, workableShifts);
        //        await client.SendAsync(email);
        //    }
        //    await client.DisconnectAsync(true);
        //
        //    return RedirectToPage();
        //}

        private List<Shift> GetWorkableShiftsFromAvailabilites(IList<Availability> availabilities)
        {
            // find all nonrecurring shifts that agree with the given availabilites
            if (_context.Shifts.Any())
            {
                List<Shift> nonRecurringShifts = _context.Shifts
                .Where(s =>
                    !(s is RecurringShift) &&
                    s.Volunteer == null &&
                    s.StartDate > DateTime.Now &&
                    availabilities
                        .Any(a =>
                            s.StartTime >= a.StartTime &&
                            s.EndTime <= a.EndTime &&
                            Enum.GetName(typeof(DayOfWeek), s.StartDate.DayOfWeek).ToLower() == a.AvailableDay)).ToList();

                // find all recurring shifts that agree with the given availabilities
                List<RecurringShift> recurringShifts = _context.RecurringShifts
                    .Where(s =>
                        s.Volunteer == null &&
                        s.EndDate > DateTime.Now &&
                        availabilities
                            .Any(a =>
                                s.StartTime >= a.StartTime &&
                                s.EndTime <= a.EndTime)).ToList();

                // merge the two lists of workable shifts
                foreach (var recurringShift in recurringShifts)
                {
                    foreach (var shift in recurringShift.ConstituentShifts)
                    {
                        if (availabilities.Any(a =>
                            a.AvailableDay == Enum.GetName(typeof(DayOfWeek), shift.StartDate.DayOfWeek).ToLower()))
                        {
                            nonRecurringShifts.Add(shift);
                        }
                    }
                }

                // order shifts by ascending date
                return nonRecurringShifts.OrderBy(s => s.StartDate).ToList();
            }
            return null;
        }

        // TODO: automate this to take place every <configured time period>
        private MimeMessage CreateEmailMessage(AppUser volunteer, List<Shift> workableShifts)
        {
            string workableShiftsStr = "";

            foreach (var shift in workableShifts)
            {
                workableShiftsStr += ">   " + shift.StartDate.ToString("dddd, dd MMMM yyyy") + " - " + shift.StartTime + " until " + shift.EndTime + "\n";
            }

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Chase", "chase.mossing2@mymhc.ca"));
            message.To.Add(new MailboxAddress(volunteer.VolunteerProfile.FirstName, volunteer.Email));
            message.Subject = "You may be able to volunteer for some new shifts!";
            message.Body = new TextPart("plain")
            {
                Text = $"Hello {volunteer.VolunteerProfile.FirstName} {volunteer.VolunteerProfile.LastName}!\n\n" +
                       $"According to your availability, you can volunteer for some currently open shifts:\n\n" +
                       workableShiftsStr + "\nIf you can attend any of these shifts, sign up for them on your online account at <websitename> or email us at <email>.\n\n" +
                       "Thanks again for volunteering at the Medicine Hat Food Bank,"
            };

            return message;
        }



        private async Task PrepareModel(string statusMessage)
        {
            // get shifts, recurring shifts and volunteers in domain model form then map them to dtos
            var volunteerDomainModels = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null).ToListAsync();
            var shiftDomainModels = _context.Shifts
                .Include(p => p.Volunteer).ThenInclude(v => v.Availabilities)
                .Include(p => p.PositionWorked)
                .Where(s => s.Hidden == false).ToList();

            ShiftMapper map = new ShiftMapper(_mapper);
            Shifts = map.MapShiftsToDtos(shiftDomainModels);

            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);

            // get positions
            Positions = _context.Positions.ToList();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");

            // update status message
            StatusMessage = statusMessage;
        }

        // convert the boolean weekday properties into the format required by rrule
        public string GetSelectedWeekdays()
        {
            string selectedDays = "";

            if (Sunday)
            {
                selectedDays += "SU,";
            }
            if (Monday)
            {
                selectedDays += "MO,";
            }
            if (Tuesday)
            {
                selectedDays += "TU,";
            }
            if (Wednesday)
            {
                selectedDays += "WE,";
            }
            if (Thursday)
            {
                selectedDays += "TH,";
            }
            if (Friday)
            {
                selectedDays += "FR,";
            }
            if (Saturday)
            {
                selectedDays += "SA,";
            }

            selectedDays = selectedDays.TrimEnd(',');

            return selectedDays;
        }

        private async Task<string> EditPosition(Position selectedPosition)
        {
            if (SelectedPosition != null)
            {
                string originalName = SelectedPosition.Name;

                _context.Update(SelectedPosition);

                SelectedPosition.Name = NewPositionName;
                await _context.SaveChangesAsync();
                return $"You successfully updated {originalName} to {SelectedPosition.Name}.";
            }
            return $"Error: You must select a position.";
        }

        private async Task<string> RemovePosition(Position selectedPosition)
        {
            if (SelectedPosition != null)
            {
                if (_context.Shifts.Any(s => s.PositionWorked == SelectedPosition))
                {
                    return $"Error: This position cannot be deleted because it is included in some of the currently scheduled shifts.";
                }
                else
                {
                    _context.Remove(SelectedPosition);
                    await _context.SaveChangesAsync();
                    return $"You successfully removed {SelectedPosition.Name} from the list of positions.";
                }
            }
            return $"You must select a position.";
        }

        private async Task<RecurringShift> EditRecurringShift(RecurringShift recShift)
        {

            if (EditType == RecurringShiftEditType.Single)
            {
                recShift = await EditSingleShiftFromRecurringSet(recShift);
            }
            else if (EditType == RecurringShiftEditType.WholeSet)
            {
                recShift = await EditWholeRecurringSet(recShift);
            }

            recShift.UpdateRecurrenceRule();

            return recShift;
        }

        private RecurringShift DeleteRecurringShift(RecurringShift recShift)
        {
            if (EditType == RecurringShiftEditType.Single)
            {
                DeleteSingleShiftFromRecurringSet(recShift);
            }
            else if (EditType == RecurringShiftEditType.WholeSet)
            {
                DeleteWholeRecurringSet(recShift);
            }

            return recShift;
        }

        private async Task<RecurringShift> EditSingleShiftFromRecurringSet(RecurringShift recShift)
        {
            // check if shift had a volunteer
            if (recShift.Volunteer != null)
            {
                // if so, cancel the original notification scheduled for it
                _reminderManager.CancelReminder(recShift, OriginalStartDate);
            }

            // make a new shift which will be excluded from the selected recurring shift, 
            // map the the selected recurring shift's properties to it, 
            // load the recurring shift's excluded shifts 
            // and add the new shift to the list of excluded shifts
            var excludedShift = new Shift();

            excludedShift = await MapShiftData(SelectedShift, excludedShift);
            // set the excluded shift's id to 0 and it's parent recurring shift to the currently selected
            // recurring shift

            if (excludedShift.Description == "vol")
            {
                return new RecurringShift() { Description = "vol" };
            }

            if (excludedShift.Description == "pos")
            {
                return new RecurringShift() { Description = "pos" };
            }

            excludedShift.Id = 0;
            excludedShift.ParentRecurringShift = _context.RecurringShifts
                .FirstOrDefault(s => s.Id == SelectedShift.Id);

            var newStartTime = excludedShift.StartTime;
            var newStartDate = excludedShift.StartDate;

            // handle all special cases
            if (recShift.StartTime != newStartTime)
            {
                // if start time has been changed, an additional shift entity with the original start time 
                // must be created and added to the recurring shift's excluded shifts so that the shift isn't duplicated
                var excludedShiftWithOriginalStartTime = new Shift
                {
                    StartTime = recShift.StartTime,
                    StartDate = OriginalStartDate,
                    Hidden = true
                };

                recShift.ExcludedShifts.Add(excludedShiftWithOriginalStartTime);

                // a link between the recurring shift, the new shift and the excluded shift must be created here
                var link = new RecurringChildLink
                {
                    ParentSet = recShift,
                    OriginalShift = excludedShiftWithOriginalStartTime,
                    NewShift = excludedShift
                };

                _context.Add(link);
                _context.Add(excludedShift);
            }
            else if (OriginalStartDate != newStartDate)
            {
                // if start date has been changed, an additional shift entity with the original start date
                // must be created and added to the recurring shift's excluded shifts so that the shift isn't duplicated
                // (same process as changed start time handling above)
                Shift excludedShiftWithOriginalStartDate = new Shift
                {
                    StartTime = recShift.StartTime,
                    StartDate = OriginalStartDate,
                    Hidden = true
                };

                recShift.ExcludedShifts.Add(excludedShiftWithOriginalStartDate);

                // a link between the recurring shift, the new shift and the excluded shift must be created here
                var link = new RecurringChildLink
                {
                    ParentSet = recShift,
                    OriginalShift = excludedShiftWithOriginalStartDate,
                    NewShift = excludedShift
                };

                _context.Add(link);
                _context.Add(excludedShift);
            }
            else
            // trivial case: something besides start date or start time has been changed
            {
                recShift.ExcludedShifts.Add(excludedShift);
            }

            // delete the recurring shift if all of its child shifts are now excluded
            bool allShiftsInRecurringSetAreExcluded = recShift.ConstituentShifts.Count == recShift.ExcludedShifts.Count;
            if (allShiftsInRecurringSetAreExcluded)
            {
                _context.Remove(recShift);
            }

            recShift.UpdateRecurrenceRule();

            await _context.SaveChangesAsync();

            // check if recurring shift has a volunteer after being edited
            if (excludedShift.Volunteer != null)
            {
                // if so, schedule a new reminder for it
                AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == excludedShift.Volunteer.Id);
                _reminderManager.ScheduleReminder(volunteerAccount.Email, excludedShift.Volunteer, excludedShift);
            }

            return recShift;
        }

        private async Task<RecurringShift> EditWholeRecurringSet(RecurringShift recShift)
        {
            // check if recurring shift had volunteer
            if (recShift.Volunteer != null)
            {
                // if so, cancel the notification scheduled for it
                _reminderManager.CancelReminder(recShift);
            }

            var initialStartTime = recShift.StartTime;

            recShift = (RecurringShift)(await MapShiftData(SelectedShift, recShift));
            // start date must be reassigned to the start date for the whole set
            recShift.StartDate = RecurrenceSetStartDate;

            var finalStartTime = recShift.StartTime;

            if (!Equals(initialStartTime, finalStartTime))
            {
                foreach (var shift in recShift.ExcludedShifts)
                {
                    shift.StartTime = recShift.StartTime;
                }

                recShift.UpdateRecurrenceRule();
            }

            await _context.SaveChangesAsync();

            // check if recurring shift has volunteer after edit
            if (recShift.Volunteer != null)
            {
                // if so, schedule a reminder for it
                AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == recShift.Volunteer.Id);
                _reminderManager.ScheduleReminder(volunteerAccount.Email, recShift.Volunteer, recShift);
            }

            return recShift;
        }

        private void DeleteSingleShiftFromRecurringSet(RecurringShift recShift)
        {
            var shift = new Shift();
            shift.StartDate = OriginalStartDate;
            shift.StartTime = recShift.StartTime;
            shift.Hidden = true;

            recShift.ExcludedShifts.Add(shift);
            recShift.UpdateRecurrenceRule();
        }

        private void DeleteWholeRecurringSet(RecurringShift recurringShift)
        {
            // find all shifts in this recurring set that have already been deleted and clear then from the database
            // all child shifts must be deleted or detached from the parent shift to avoid violating the foreign key constraint
            foreach (var shift in recurringShift.ExcludedShifts)
            {
                bool shiftHasBeenDeleted = shift.Hidden;
                if (shiftHasBeenDeleted)
                {
                    _context.Remove(shift);
                }
                else
                {
                    // if it hasn't been deleted from the set. let it remain and set its parentrecurringshiftproperty to null
                    _context.Update(shift);
                    shift.ParentRecurringShift = null;
                }
            }

            recurringShift.ExcludedShifts.Clear();
            _context.Shifts.Remove(recurringShift);

            // check if shift had a volunteer
            if (recurringShift.Volunteer != null)
            {
                // if so, cancel the notification scheduled for it
                _reminderManager.CancelReminder(recurringShift);
            }
        }
    }
}
