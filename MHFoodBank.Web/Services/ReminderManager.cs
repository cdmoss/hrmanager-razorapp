using Hangfire;
using MailKit.Net.Smtp;
using MHFoodBank.Common;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Data
{
    public interface IReminderManager
    {
        void ScheduleReminder(string email, VolunteerProfile volunteer, Shift shift, DateTime? shiftDate = null);
        void CancelReminder(Shift shift, DateTime? shiftDate = null);
        void SendEmail(string email, string firstName, string lastName, string startTime, string endTime, string position);
        MimeMessage CreateEmail(string email, string firstName, string lastName, string startTime, string endTime, string position);
    }


    public class ReminderManager : IReminderManager
    {
        private readonly FoodBankContext _context;

        public ReminderManager(FoodBankContext context)
        {
            _context = context;
        }

        // in the instance that a single shift from a recurring set needs to have its reminder scheduled
        // (e.g. when a shift is having its original properties restored),
        // the datetime of the selected shift will be passed in
        public void ScheduleReminder(string email, VolunteerProfile volunteer, Shift shift, DateTime? shiftDate = null)
        {
            if (shift is RecurringShift recurringShift)
            {
                if (shiftDate == null)
                {
                    foreach (var constituentShift in recurringShift.ConstituentShifts)
                    {
                        var idForRecurring = BackgroundJob.Schedule(() =>
                            SendEmail(email,
                                volunteer.FirstName,
                                volunteer.LastName,
                                shift.StartTime.ToString(),
                                shift.EndTime.ToString(),
                                shift.PositionWorked.Name),
                            constituentShift.StartDate.AddHours(-12));

                        _context.Add(new Reminder() { ShiftId = recurringShift.Id, ShiftDate = constituentShift.StartDate, HangfireJobId = idForRecurring });
                    }
                }
                else
                {
                    var id = BackgroundJob.Schedule(() =>
                    SendEmail(
                        email,
                        volunteer.FirstName,
                        volunteer.LastName,
                        shift.StartTime.ToString(),
                        shift.EndTime.ToString(),
                        shift.PositionWorked.Name),
                    ((DateTime)shiftDate).AddHours(-12));

                    _context.Add(new Reminder() { ShiftId = shift.Id, ShiftDate = shift.StartDate, HangfireJobId = id });
                }
            }
            else
            {
                var id = BackgroundJob.Schedule(() => 
                    SendEmail(
                        email, 
                        volunteer.FirstName, 
                        volunteer.LastName, 
                        shift.StartTime.ToString(), 
                        shift.EndTime.ToString(), 
                        shift.PositionWorked.Name), 
                    shift.StartDate.AddHours(-12));

                _context.Add(new Reminder() { ShiftId = shift.Id, ShiftDate = shift.StartDate, HangfireJobId = id });
            }
        }

        // in the instance that a single shift from a recurring set needs to have its reminder canceled,
        // the datetime of the selected shift will be passed in
        public void CancelReminder(Shift shift, DateTime? shiftDate = null)
        {
            if (shift is RecurringShift recurringShift)
            {
                if (shiftDate == null)
                {
                    List<Reminder> remindersForRecurringShift = _context.Reminders.Where(r => r.ShiftId == recurringShift.Id).ToList();
                    foreach (var reminder in remindersForRecurringShift)
                    {
                        BackgroundJob.Delete(reminder.HangfireJobId);
                        _context.Reminders.Remove(reminder);
                    }
                }
                else
                {
                    Reminder reminderForSelectedShift = _context.Reminders.FirstOrDefault(r => r.ShiftId == recurringShift.Id && r.ShiftDate == shiftDate);
                    BackgroundJob.Delete(reminderForSelectedShift.HangfireJobId);
                    _context.Reminders.Remove(reminderForSelectedShift);
                }
            }
            else
            {
                var reminder = _context.Reminders.FirstOrDefault(r => r.ShiftId == shift.Id && r.ShiftDate == shift.StartDate);
                BackgroundJob.Delete(reminder.HangfireJobId);
                _context.Reminders.Remove(reminder);
            }
        }

        public void SendEmail(string email, string firstName, string lastName, string startTime, string endTime, string position)
        {
            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587);
            client.Authenticate("chase.mossing2@mymhc.ca", "Mar1995303");
            var message = CreateEmail(email, firstName, lastName, startTime, endTime, position);
            client.Send(message);
            client.Disconnect(true);
        }

        public MimeMessage CreateEmail(string email, string firstName, string lastName, string startTime, string endTime, string position)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Chase", "chase.mossing2@mymhc.ca"));
            emailMessage.To.Add(new MailboxAddress(firstName, email));
            emailMessage.Subject = "A friendly reminder that you volunteer at the Medicine Hat Food Bank tomorrow!";
            emailMessage.Body = new TextPart("plain")
            {
                Text = $"Hello {firstName} {lastName}!\n\n" +
                       $"We are reminding you that are scheduled to volunteer at Medicine Hat Food Bank tomorrow. " +
                       $"The shift is scheduled from {startTime} until {endTime} and the position is {position}.\n\n" +
                       "Thanks again for volunteering at the Medicine Hat Food Bank."
            };

            return emailMessage;
        }
    }
}
