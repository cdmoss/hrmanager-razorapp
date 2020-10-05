using Hangfire;
using MailKit.Net.Smtp;
using MHFoodBank.Common;
using MHFoodBank.Common.Services;
using MHFoodBank.Web.Services;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Data
{
    public interface IReminderManager
    {
        void ScheduleReminder(AppUser volunteer, Shift shift);
        void CancelReminder(Shift shift, DateTime? shiftDate = null);
        string CreateEmail(AppUser user, Shift shift);
    }


    public class ReminderManager : IReminderManager
    {
        private readonly FoodBankContext _context;
        private readonly ILogger<ReminderManager> _logger;
        private readonly IEmailSender _emailSender;

        public ReminderManager(FoodBankContext context, ILogger<ReminderManager> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        // in the instance that a single shift from a recurring set needs to have its reminder scheduled
        // (e.g. when a shift is having its original properties restored),
        // the datetime of the selected shift will be passed in
        public void ScheduleReminder(AppUser volunteer, Shift shift)
        {
            _context.Entry(volunteer).Reference(v => v.VolunteerProfile).Load();
            if (!shift.IsRecurrence)
            {
                // AddHours(-6) instead of -12 corrects the hangfire UTC conversion
                var id = BackgroundJob.Schedule(() =>
                _emailSender.SendEmailAsync(volunteer.Email, "Volunteering Reminder - MHFB", CreateEmail(volunteer, shift)),
                shift.StartTime.Date.AddHours(-6));

                _context.Add(new Reminder() { ShiftId = shift.Id, ShiftDate = shift.StartTime, HangfireJobId = id });
            }
            else
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
                        var idForRecurring = BackgroundJob.Schedule(() =>
                        _emailSender.SendEmailAsync(volunteer.Email, "Volunteering Reminder - MHFB", CreateEmail(volunteer, shift)),
                        date.AddHours(-6));

                        _context.Add(new Reminder() { ShiftId = shift.Id, ShiftDate = date, HangfireJobId = idForRecurring });
                    }
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
                            BackgroundJob.Delete(reminderForSelectedShift.HangfireJobId);
                            _context.Reminders.Remove(reminderForSelectedShift);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        List<Reminder> remindersForRecurringShift = _context.Reminders.Where(r => r.ShiftId == shift.Id).ToList();
                        foreach (var reminder in remindersForRecurringShift)
                        {
                            BackgroundJob.Delete(reminder.HangfireJobId);
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
                        BackgroundJob.Delete(reminder.HangfireJobId);
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
