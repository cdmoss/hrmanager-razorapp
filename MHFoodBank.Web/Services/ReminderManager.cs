using Hangfire;
using MHFoodBank.Common;
using MHFoodBank.Common.Services;
using MHFoodBank.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Data
{
    public interface IReminderManager
    {
        bool ScheduleReminder(Shift shift);
        void CancelReminder(Shift shift, DateTime? shiftDate = null);
        string CreateEmail(AppUser user, Shift shift);
    }


    public class ReminderManager : IReminderManager
    {
        private readonly FoodBankContext _context;
        private readonly ILogger<ReminderManager> _logger;
        private readonly IEmailSender _emailSender;
        bool _isTesting;

        public ReminderManager(FoodBankContext context, ILogger<ReminderManager> logger, IEmailSender emailSender, bool isTesting = false)
        {
            _isTesting = isTesting;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        // in the instance that a single shift from a recurring set needs to have its reminder scheduled
        // (e.g. when a shift is having its original properties restored),
        // the datetime of the selected shift will be passed in
        public bool ScheduleReminder(Shift shift)
        {
            try
            {
                //schedule email notification for shift if shift is after todays date if it's not an open shift
                if (shift.VolunteerProfileId != null || shift.VolunteerProfileId > 0)
                {
                    // if shift is recurring, check each child shift to see if the recurrence period requires reminders
                    if (shift.IsRecurrence)
                    {
                        var recurrenceDates = RecurrenceHelper.GetRecurrenceDateTimeCollection(shift.RecurrenceRule, shift.StartTime);
                        if (recurrenceDates.Any(d => d > DateTime.Now.AddDays(1).Date))
                        {
                            var volunteerAccount = _context.Users.FirstOrDefault(u => u.VolunteerProfile.Id == shift.VolunteerProfileId);
                            ScheduleReminderForRecurringShifts(volunteerAccount, shift);
                        }
                    }
                    // if it isn't recurring then simply schedule a reminder
                    else if (shift.StartTime > DateTime.Now.AddDays(1).Date)
                    {
                        var volunteerAccount = _context.Users.FirstOrDefault(u => u.VolunteerProfile.Id == shift.VolunteerProfileId);
                        ScheduleReminderForSingleShift(volunteerAccount, shift);
                    }
                }

                _logger.LogInformation($"A new reminder was successfully scheduled for shift {shift.Id} on {shift.StartTime.Date.AddHours(-6)}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to schedule a new reminder for shift {shift.Id} on {shift.StartTime.Date.AddHours(-6)} \n Error Message: {ex.Message}");
                return true;
            }
        }

        private void ScheduleReminderForSingleShift(AppUser volunteer, Shift shift)
        {
            _context.Entry(volunteer).Reference(v => v.VolunteerProfile).Load();
            // AddHours(-6) instead of -12 corrects the hangfire UTC conversion
            if (!_isTesting)
            {
                var id = BackgroundJob.Schedule(() =>
                _emailSender.SendEmailAsync(volunteer.Email, "Volunteering Reminder - MHFB", CreateEmail(volunteer, shift)),
                shift.StartTime.Date.AddHours(-6));
            }

            _context.SaveChanges();
        }

        private async Task ScheduleReminderForRecurringShifts(AppUser volunteer, Shift shift)
        {
            // then schedule a reminder for each child shift that needs one
            var childShiftDates = RecurrenceHelper.GetRecurrenceDateTimeCollection(shift.RecurrenceRule, shift.StartTime);
            foreach (var date in childShiftDates)
            {
                // only schedule a reminder if the date is the next day or after, and if there isn't already a scheduled reminder
                // there might already be a scheduled reminder since the caller might removing a shift from a recurring set
                bool isFutureShift = date > DateTime.Now.AddDays(1).Date;
                bool noScheduledReminder = !_context.Reminders.Any(r => r.ShiftId == shift.Id && r.ShiftDate == date);
                if (isFutureShift && noScheduledReminder)
                {
                    string idForRecurring = "";
                    if (!_isTesting)
                    {
                        idForRecurring = BackgroundJob.Schedule(() =>
                        _emailSender.SendEmailAsync(volunteer.Email, "Volunteering Reminder - MHFB", CreateEmail(volunteer, shift)),
                        date.AddHours(-6));
                    }

                    var reminders = await _context.Reminders.ToListAsync();

                    idForRecurring = (reminders.Max(p => p.Id) + 1).ToString();

                    _context.Add(new Reminder() { ShiftId = shift.Id, ShiftDate = date, HangfireJobId = idForRecurring });
                }
            }

            _context.SaveChanges();
        }

        // in the instance that a single shift from a recurring set needs to have its reminder canceled,
        // the datetime of the selected shift will be passed in.
        // If no datetime is passed in and the shift is a recurring shift, all shifts associated with it will be deleted
        public void CancelReminder(Shift shift, DateTime? shiftDate = null)
        {
            try
            {
                if (shift.IsRecurrence)
                {
                    bool cancellingSingleShift = shiftDate != null;
                    if (cancellingSingleShift)
                    {
                        var reminderForSelectedShift = _context.Reminders.FirstOrDefault(r => r.ShiftId == shift.Id && r.ShiftDate == shiftDate);
                        if (reminderForSelectedShift != null)
                        {
                            if (!_isTesting)
                            {
                                BackgroundJob.Delete(reminderForSelectedShift.HangfireJobId);
                            }
                            _context.Reminders.Remove(reminderForSelectedShift);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        List<Reminder> remindersForRecurringShift = _context.Reminders.Where(r => r.ShiftId == shift.Id).ToList();
                        foreach (var reminder in remindersForRecurringShift)
                        {
                            if (!_isTesting)
                            {
                                BackgroundJob.Delete(reminder.HangfireJobId);
                            }
                            _context.Reminders.Remove(reminder);
                            _context.SaveChanges();
                        }
                    }
                }
                else
                {
                    Reminder reminder;
                    if (shift.RecurrenceID != null)
                    {
                        reminder = _context.Reminders.FirstOrDefault(r => r.ShiftId == shift.Id && r.ShiftDate == shiftDate);
                    }
                    else
                    {
                        reminder = _context.Reminders.FirstOrDefault(r => r.ShiftId == shift.Id && r.ShiftDate == shift.StartTime);
                    }

                    if (reminder != null)
                    {
                        if (!_isTesting)
                        {
                            BackgroundJob.Delete(reminder.HangfireJobId);
                        }
                        _context.Reminders.Remove(reminder);
                        _context.SaveChanges();
                    }
                    else
                    {
                        int shiftId = shift.Id;
                        _logger.LogWarning($"A reminder for that shift (ShiftID = {shiftId}) was not found");
                    }
                }
            }
            catch (Exception ex)
            {
                int shiftId = shift.Id;
                _logger.LogError($"There was an error when attempting to cancel a shift reminder. \n\n Shift ID = {shiftId} \n\n {ex.Message}");
            }
        }



        public string CreateEmail(AppUser user, Shift shift)
        {
            return $"Hello {user.VolunteerProfile.FirstName} {user.VolunteerProfile.LastName}!\n\n" +
                       $"We are reminding you that are scheduled to volunteer at Medicine Hat Food Bank tomorrow. " +
                       $"The shift is scheduled from {shift.StartTime.TimeOfDay} until {shift.EndTime.TimeOfDay} and the position is {shift.Position.Name}.\n\n" +
                       "Thanks again for volunteering at the Medicine Hat Food Bank.";
        }
    }
}
