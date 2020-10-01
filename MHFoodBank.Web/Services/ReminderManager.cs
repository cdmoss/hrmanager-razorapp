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
        Task ScheduleReminder(AppUser volunteer, Shift shift);
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
        public async Task ScheduleReminder(AppUser volunteer, Shift shift)
        {
            await _context.Entry(volunteer).Reference(v => v.VolunteerProfile).LoadAsync();
            if (!shift.IsRecurrence)
            {
                var id = BackgroundJob.Schedule(() =>
                _emailSender.SendEmailAsync(volunteer.Email, "Volunteering Reminder - MHFB", CreateEmail(volunteer, shift)),
                shift.StartTime.AddHours(-12));

                _context.Add(new Reminder() { ShiftId = shift.Id, ShiftDate = shift.StartTime, HangfireJobId = id });
            }
            else
            {
                var childShiftDates = RecurrenceHelper.GetRecurrenceDateTimeCollection(shift.RecurrenceRule, shift.StartTime);
                foreach (var date in childShiftDates)
                {
                    var idForRecurring = BackgroundJob.Schedule(() =>
                    _emailSender.SendEmailAsync(volunteer.Email, "Volunteering Reminder - MHFB", CreateEmail(volunteer, shift)),
                    shift.StartTime.AddHours(-12));

                    _context.Add(new Reminder() { ShiftId = shift.Id, ShiftDate = date, HangfireJobId = idForRecurring });
                }
            }
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
                    List<Reminder> remindersForRecurringShift = _context.Reminders.Where(r => r.ShiftId == shift.Id).ToList();
                    foreach (var reminder in remindersForRecurringShift)
                    {
                        BackgroundJob.Delete(reminder.HangfireJobId);
                        _context.Reminders.Remove(reminder);
                    }
                }
                else
                {
                    Reminder reminder;
                    if (shift.IsRecurrence)
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
                       $"The shift is scheduled from {shift.StartTime} until {shift.EndTime} and the position is {shift.Position}.\n\n" +
                       "Thanks again for volunteering at the Medicine Hat Food Bank.";
        }
    }
}
