using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using WorkplaceAdministrator.Web.Areas.Admin.Pages.Shared;
using WorkplaceAdministrator.Web.Data;
using WorkplaceAdministrator.Web.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Hangfire.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using WorkplaceAdministrator.Common.Dtos;
using AutoMapper;

namespace WorkplaceAdministrator.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin" )]
    public class AdminCalendar : AdminPageModel
    {
        public enum RecurringShiftEditType
        {
            Single,
            WholeSet
        }

        [BindProperty]
        public Position Position { get; set; }
        [BindProperty]
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        [BindProperty]
        public List<ShiftReadEditDto> Shifts { get; set; }
        public List<RecurringShiftReadEditDto> RecurringShifts { get; set; }
        [BindProperty]
        public List<Position> Positions { get; set; }
        [BindProperty(SupportsGet = true)]

        // the position displayed on page load
        public Position DefaultPosition { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }
        [BindProperty]
        public Position SearchedPosition { get; set; }
        [BindProperty]
        public Position PositionToRemove { get; set; }
        public string StatusMessage { get; set; }
        [BindProperty]
        public RecurringShiftEditType EditType { get; set; }

        #region weekdays for recurring shift
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

        private readonly IMapper _mapper;

        public AdminCalendar(FoodBankContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task OnGet(string statusMessage = null)
        {
            // get shifts, recurring shifts and volunteers
            var volunteerDomainModels = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null).ToListAsync();
            var shiftDomainModels = _context.Shifts
                .Include(p => p.Volunteer).ThenInclude(v => v.Availabilities)
                .Include(p => p.PositionWorked)
                .Where(s => s.Hidden == false && !(s is RecurringShift)).ToList();
            var recurringShiftDomainModels = await _context.RecurringShifts
                .Include(p => p.Volunteer).ThenInclude(v => v.Availabilities)
                .Include(p => p.PositionWorked)
                .Where(s => s.Hidden == false).ToListAsync();

            Shifts = _mapper.Map<List<ShiftReadEditDto>>(shiftDomainModels);
            RecurringShifts = _mapper.Map<List<RecurringShiftReadEditDto>>(recurringShiftDomainModels);

            foreach (var recurringShift in RecurringShifts)
            {
                Shifts.Add(recurringShift);
            }

            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);

            Positions = _context.Positions.ToList();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");

            StatusMessage = statusMessage;
        }

        public async Task<IActionResult> OnPostAddPosition()
        {
            // add the shift
            string positionName = Request.Form["add-position-name"];
            Position position = new Position { Name = positionName };
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();

            // update status message
            StatusMessage = $"You successfully added the {position.Name} to the list of positions.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditPosition()
        {
            PositionToRemove = _context.Positions.FirstOrDefault(p => p.Id == Convert.ToInt32(Request.Form["edit-position-original"]));
            if (PositionToRemove != null)
            {
                string originalName = PositionToRemove.Name;

                _context.Update(PositionToRemove);
                string newName = Request.Form["edit-position-name"];
                if (newName != "")
                {
                    PositionToRemove.Name = newName;
                    await _context.SaveChangesAsync();
                    StatusMessage = $"You successfully updated {originalName} to {PositionToRemove.Name}.";
                }
                else
                {
                    return RedirectToPage(new { statusMessage = $"Error: you must enter a name for the new position." });
                }
                
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemovePosition()
        {
            PositionToRemove = _context.Positions.FirstOrDefault(p => p.Id == Convert.ToInt32(Request.Form["edit-position-original"]));
            if (PositionToRemove != null)
            {
                if (_context.Shifts.Any(s => s.PositionWorked == PositionToRemove))
                {
                    return RedirectToPage(new { statusMessage = $"Error: This position cannot be deleted because it is included in some of the currently scheduled shifts." });
                }
                else
                {
                    _context.Remove(PositionToRemove);
                    await _context.SaveChangesAsync();
                    return RedirectToPage(new { statusMessage = $"You successfully removed {PositionToRemove.Name} from the list of positions." });
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddShift()
        {
            Shift shift = new Shift();
            GetDataToAddShift(shift);
            shift.CreateDescription();
            await _context.Shifts.AddAsync(shift);
            await _context.SaveChangesAsync();

            // schedule email notification for shift
            if (shift.Volunteer != null)
            {
                AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
                ReminderScheduler.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift, _context);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddRecurringShift()
        {
            RecurringShift recurringShift = new RecurringShift();
            GetDataToAddShift(recurringShift);
            recurringShift.CreateDescription();
            recurringShift.EndDate = Convert.ToDateTime(Request.Form["add-recshift-enddate"]);
            recurringShift.Weekdays = GetSelectedWeekdays();
            recurringShift.UpdateRecurrenceRule();

            _context.RecurringShifts.Add(recurringShift);
            await _context.SaveChangesAsync();

            // schedule email notification for shift
            if (recurringShift.Volunteer != null)
            {
                AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == recurringShift.Volunteer.Id);
                ReminderScheduler.ScheduleReminder(volunteerAccount.Email, recurringShift.Volunteer, recurringShift, _context);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditShift()
        {
            Shift shift = _context.Shifts.Include(p => p.Volunteer).FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-shift-id"]));

            _context.Shifts.Update(shift);

            // this method will handle scheduling reminders
            await GetDataToEditShift(shift);

            shift.CreateDescription();
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditRecurringShift()
        {
            Shift shift = _context.Shifts
                .Include(p => p.Volunteer)
                .Include(p => p.PositionWorked)
                .FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-recshift-id"]));
            _context.Shifts.Update(shift);

            // this method will handle scheduling reminders
            await GetDataToEditShift(shift);

            shift.CreateDescription();
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = "You have successfully edited the chosen shift." });
        }

        public async Task<IActionResult> OnPostDeleteShift()
        {
            Shift shift = _context.Shifts.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-shift-id"]));
            await _context.Entry(shift).Reference(p => p.Volunteer).LoadAsync();
            List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
                .Include(p => p.NewShift)
                .Include(p => p.OldShift)
                .Where(a => a.NewShift == shift || a.OldShift == shift)
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
                        ReminderScheduler.CancelReminder(shift, _context);
                    }

                    _context.Shifts.Remove(shift);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage(new { statusMessage = StatusMessage });
        }

        public async Task<IActionResult> OnPostDeleteShiftFromRecurringSet()
        {
            Shift shift = _context.Shifts.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-recshift-single-confirm-id"]));
            List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
                .Include(p => p.NewShift)
                .Include(p => p.OldShift)
                .Where(a => a.NewShift == shift || a.OldShift == shift)
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
                        ReminderScheduler.CancelReminder(shift, _context);
                    }

                    // shift is hidden from the calendar but will stay in the 
                    // excludedshifts list of it's parent recurring shift
                    shift.Hidden = true;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteRecurringShift()
        {
            RecurringShift recurringShift = _context.RecurringShifts.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-recshift-id"]));
            await _context.Entry(recurringShift).Collection(p => p.ExcludedShifts).LoadAsync();
            _context.Update(recurringShift);
            List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
                .Include(p => p.NewShift)
                .Include(p => p.OldShift)
                .Where(a => a.NewShift == recurringShift || a.OldShift == recurringShift)
                .ToListAsync();

            if (recurringShift != null)
            {
                if (alertsWithChosenShift.Any())
                {
                    return RedirectToPage(new { statusMessage = $"Error: The shift you tried to delete is part of an existing shift request. The shift request must be addressed before the shift can be deleted." });
                }
                else
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

                    await _context.Entry(recurringShift).Reference(p => p.Volunteer).LoadAsync();
                    // check if shift had a volunteer
                    if (recurringShift.Volunteer != null)
                    {
                        // if so, cancel the notification scheduled for it
                        ReminderScheduler.CancelReminder(recurringShift, _context);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRestoreShiftPropertiesToOriginal()
        {
            Shift shift = _context.Shifts.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-recshift-single-confirm-id"]));
            await _context.Entry(shift).Reference(p => p.ParentRecurringShift).LoadAsync();
            await _context.Entry(shift.ParentRecurringShift).Collection(p => p.ExcludedShifts).LoadAsync();
            await _context.Entry(shift.ParentRecurringShift).Reference(p => p.Volunteer).LoadAsync();
            await _context.Entry(shift.ParentRecurringShift).Reference(p => p.PositionWorked).LoadAsync();
            List<ShiftRequestAlert> alertsWithChosenShift = await _context.ShiftAlerts
                .Include(p => p.NewShift)
                .Include(p => p.OldShift)
                .Where(a => a.NewShift == shift || a.OldShift == shift)
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
                        ReminderScheduler.CancelReminder(shift, _context, shift.StartDate);
                    }

                    _context.Shifts.Remove(shift);
                    _context.Update(shift.ParentRecurringShift);
                    shift.ParentRecurringShift.ExcludedShifts.Remove(shift);
                    shift.ParentRecurringShift.UpdateRecurrenceRule();

                    // check if parent recurring shift has volunteer
                    if (shift.ParentRecurringShift.Volunteer != null)
                    {
                        // schedule email notification for shift
                        AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.ParentRecurringShift.Volunteer.Id);
                        ReminderScheduler.ScheduleReminder(volunteerAccount.Email, shift.ParentRecurringShift.Volunteer, shift.ParentRecurringShift, _context, shift.StartDate);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage(new { statusMessage = "You have successfully returned the chosen shift's properties to their original state."});
        }

        public async Task OnPostSearch()
        {
            await PrepareModel(null);

            Searcher searcher = new Searcher(_context);
            SearchedPosition = searcher.GetSearchedPosition(Request);
            Shifts = searcher.FilterShiftsBySearch(Shifts, SearchedName, SearchedPosition);
        }

        public async Task<IActionResult> OnPostSendNotifications()
        {
            await PrepareModel(null);
            List<AppUser> volunteersUserAccounts = await _context.Users.Where(u => u.VolunteerProfile != null).ToListAsync();

            SmtpClient client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587);
            await client.AuthenticateAsync("chase.mossing2@mymhc.ca", "Mar1995303");

            if (!(Shifts.Any() || RecurringShifts.Any()))
            {
                return RedirectToPage();
            }

            foreach (var volunteer in volunteersUserAccounts)
            {
                await _context.Entry(volunteer.VolunteerProfile).Collection(p => p.Availabilities).LoadAsync();
                List<Shift> workableShifts = GetWorkableShiftsFromAvailabilites(volunteer.VolunteerProfile.Availabilities);
                bool noWorkableShifts = !workableShifts.Any();

                if (noWorkableShifts)
                {
                    continue;
                }
                MimeMessage email = CreateEmail(volunteer, workableShifts);
                await client.SendAsync(email);
            }
            await client.DisconnectAsync(true);

            return RedirectToPage();
        }

        private List<Shift> GetWorkableShiftsFromAvailabilites(IList<Availability> availabilities)
        {
            // finds all nonrecurring shifts that fall within this volunteer's availability
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

                // finds all recurring shifts that fall within this volunteer's availability
                List<RecurringShift> recurringShifts = _context.RecurringShifts
                    .Where(s =>
                        s.Volunteer == null &&
                        s.EndDate > DateTime.Now &&
                        availabilities
                            .Any(a =>
                                s.StartTime >= a.StartTime &&
                                s.EndTime <= a.EndTime)).ToList();

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

                return nonRecurringShifts.OrderBy(s => s.StartDate).ToList();
            }
            return null;
        }

        private MimeMessage CreateEmail(AppUser volunteer, List<Shift> workableShifts)
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
            var volunteerDomainModels = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null).ToListAsync();
            var shiftDomainModels = _context.Shifts
                .Include(p => p.Volunteer).ThenInclude(v => v.Availabilities)
                .Include(p => p.PositionWorked)
                .Where(s => s.Hidden == false).ToList();
            var recurringShiftDomainModels = await _context.RecurringShifts
                .Include(p => p.Volunteer).ThenInclude(v => v.Availabilities)
                .Include(p => p.PositionWorked)
                .Where(s => s.Hidden == false).ToListAsync();

            Shifts = _mapper.Map<List<ShiftReadEditDto>>(shiftDomainModels);
            RecurringShifts = _mapper.Map<List<RecurringShiftReadEditDto>>(recurringShiftDomainModels);
            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);

            Positions = _context.Positions.ToList();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");

            StatusMessage = statusMessage;
        }

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

        private void GetDataToAddShift(Shift shift)
        {
            shift.StartDate = Convert.ToDateTime(Request.Form["add-shift-date"]);
            shift.StartTime = TimeSpan.Parse(Request.Form["add-shift-starttime"]);
            shift.EndTime = TimeSpan.Parse(Request.Form["add-shift-endtime"]);
            shift.PositionWorked = _context.Positions.FirstOrDefault(p => p.Id == Convert.ToInt32(Request.Form["add-shift-position"]));
            shift.Volunteer = _context.VolunteerProfiles.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["add-shift-volunteer"]));
        }

        private async Task GetDataToEditShift(Shift shift)
        {
            _context.Update(shift);
            if (shift is RecurringShift recShift)
            {
                if (EditType == RecurringShiftEditType.Single)
                {
                    DateTime selectedDate = Convert.ToDateTime(Request.Form["edit-recshift-single-startdate"]);

                    // check if shift had a volunteer
                    if (recShift.Volunteer != null)
                    {
                        // if so, cancel the notification scheduled for it
                        ReminderScheduler.CancelReminder(shift, _context, selectedDate);
                    }

                    // make the new shift and exclude it from recshift
                    await _context.Entry(recShift).Collection(p => p.ExcludedShifts).LoadAsync();
                    Shift excludedShift = new Shift()
                    {
                        StartDate = Convert.ToDateTime(Request.Form["edit-recshift-single-startdate"]),
                        StartTime = TimeSpan.Parse(Request.Form["edit-recshift-starttime"]),
                        EndTime = TimeSpan.Parse(Request.Form["edit-recshift-endtime"]),
                        Volunteer = _context.VolunteerProfiles.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-recshift-volunteer"])),
                        PositionWorked = _context.Positions.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-recshift-position"]))
                    };
                    excludedShift.CreateDescription();
                    
                    recShift.ExcludedShifts.Add(excludedShift);

                    // delete recshift if all of its shifts are excluded
                    bool allShiftsInRecurringSetAreExcluded = recShift.ConstituentShifts.Count == recShift.ExcludedShifts.Count;

                    if (allShiftsInRecurringSetAreExcluded)
                    {
                        _context.Remove(recShift);
                    }

                    await _context.SaveChangesAsync();

                    // check if shift has a volunteer after edit
                    if (excludedShift.Volunteer != null)
                    {
                        // if so, schedule a reminder for it
                        AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == excludedShift.Volunteer.Id);
                        ReminderScheduler.ScheduleReminder(volunteerAccount.Email, excludedShift.Volunteer, excludedShift, _context);
                    }
                }
                else if (EditType == RecurringShiftEditType.WholeSet)
                {
                    // check if recurring shift had volunteer
                    if (recShift.Volunteer != null)
                    {
                        // if so, cancel the notification scheduled for it
                        ReminderScheduler.CancelReminder(shift, _context);
                    }

                    recShift.StartDate = Convert.ToDateTime(Request.Form["edit-recshift-all-startdate"]);
                    recShift.StartTime = TimeSpan.Parse(Request.Form["edit-recshift-starttime"]);
                    recShift.EndTime = TimeSpan.Parse(Request.Form["edit-recshift-endtime"]);
                    recShift.Volunteer = _context.VolunteerProfiles.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-recshift-volunteer"]));
                    recShift.PositionWorked = _context.Positions.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-recshift-position"]));
                    recShift.EndDate = Convert.ToDateTime(Request.Form["edit-recshift-enddate"]);
                    recShift.Weekdays = GetSelectedWeekdays();

                    // check if recurring shift has volunteer after edit
                    if (recShift.Volunteer != null)
                    {
                        // if so, schedule a reminder for it
                        AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == recShift.Volunteer.Id);
                        ReminderScheduler.ScheduleReminder(volunteerAccount.Email, recShift.Volunteer, recShift, _context);
                    }
                }

                recShift.UpdateRecurrenceRule();
            }
            else
            {
                // check if shift had a volunteer
                if (shift.Volunteer != null)
                {
                    // if so, cancel the notification scheduled for it
                    ReminderScheduler.CancelReminder(shift, _context);
                }

                shift.StartDate = Convert.ToDateTime(Request.Form["edit-shift-date"]);
                shift.StartTime = TimeSpan.Parse(Request.Form["edit-shift-starttime"]);
                shift.EndTime = TimeSpan.Parse(Request.Form["edit-shift-endtime"]);
                shift.Volunteer = _context.VolunteerProfiles.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-shift-volunteer"]));
                shift.PositionWorked = _context.Positions.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Form["edit-shift-position"]));

                // check if shift has volunteer after edit
                if (shift.Volunteer != null)
                {
                    // if so, schedule a reminder for it
                    AppUser volunteerAccount = await _context.Users.FirstOrDefaultAsync(u => u.VolunteerProfile.Id == shift.Volunteer.Id);
                    ReminderScheduler.ScheduleReminder(volunteerAccount.Email, shift.Volunteer, shift, _context);
                }
            }
        }
    }
}
