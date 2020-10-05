using MHFoodBank.Common;
using MHFoodBank.Common.Services;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Services
{
    public interface IEmailAvailableShiftService
    {
        Task SendNotifications();
    }

    public class EmailAvailableShiftService : IEmailAvailableShiftService
    {
        private readonly FoodBankContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailAvailableShiftService> _logger;

        public EmailAvailableShiftService(FoodBankContext context, IEmailSender emailSender, ILogger<EmailAvailableShiftService> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task SendNotifications()
        {
            // if there are no shifts in db, redirect immediately
            if (!_context.Shifts.Any())
            {
                //return RedirectToPage();
            }

            // get all volunteers
            List<AppUser> volunteersUserAccounts = await _context.Users.Where(u => u.VolunteerProfile != null).Include(x => x.VolunteerProfile).ThenInclude(x => x.Availabilities).ToListAsync();

            // for each volunteer, prepare a list of shifts that agrees with their availability
            foreach (var volunteer in volunteersUserAccounts)
            {
                // if this volunteer has no availabilities, skip them
                if (!volunteer.VolunteerProfile.Availabilities.Any())
                {
                    continue;
                }

                await _context.Entry(volunteer.VolunteerProfile).Collection(p => p.Availabilities).LoadAsync();
                List<Shift> workableShifts = GetWorkablShiftsForVolunteer(volunteer.VolunteerProfile);
                bool noWorkableShifts = !workableShifts.Any();

                // skip the volunteer if they can't work any shifts
                if (noWorkableShifts)
                {
                    continue;
                }

                string workableShiftsStr = "";

                foreach (var shift in workableShifts)
                {
                    workableShiftsStr += ">   " + shift.Position.Name + ", " + shift.StartTime.ToString("dddd, dd MMMM yyyy") + ", " + shift.StartTime.TimeOfDay + " until " + shift.EndTime.TimeOfDay + "<br/>";
                }

                await _emailSender.SendEmailAsync(volunteer.Email, "Available open shifts - MHFB", $"Hello { volunteer.VolunteerProfile.FirstName} { volunteer.VolunteerProfile.LastName},<br/><br/>"
                 + $"According to your availability, you can volunteer for some currently open shifts:<br/><br/>" +
                           workableShiftsStr + "</br></br>If you can attend any of these shifts, sign up for them on your <a href='https://volunteer.mhfoodbank.com'>online account</a> at or <a href='mailto:schedule@mhfoodbank.com'>email</a> us at schedule@mhfoodbank.com.<br/><br/>" +
                           "Thanks again for volunteering at the Medicine Hat Food Bank.");

            }
        }

        public List<Shift> GetWorkablShiftsForVolunteer(VolunteerProfile volunteer)
        {
            // find all nonrecurring shifts that agree with the given availabilites
            if (_context.Shifts.Any())
            {
                _context.Entry(volunteer).Collection(v => v.Availabilities).Load();
                _context.Entry(volunteer).Reference(v => v.Positions).Load();

                List<Shift> availableShifts = new List<Shift>();

                foreach (var shift in _context.Shifts.Where(s => string.IsNullOrEmpty(s.RecurrenceRule)))
                {
                    if (shift.Volunteer == null &&
                    shift.StartTime > DateTime.Now &&
                    volunteer.Availabilities
                        .Any(a =>
                            shift.StartTime.TimeOfDay >= a.StartTime &&
                            shift.EndTime.TimeOfDay <= a.EndTime &&
                            Enum.GetName(typeof(DayOfWeek), shift.StartTime.DayOfWeek).ToLower() == a.AvailableDay) &&
                            volunteer.Positions.Any(p => p.Id == shift.PositionId))
                    {
                        availableShifts.Add(shift);
                    }
                }

                // find all recurring shifts that agree with the given availabilities
                foreach (var recurringShift in _context.Shifts.Where(s => !string.IsNullOrWhiteSpace(s.RecurrenceRule)))
                {
                    var childShiftDates = RecurrenceHelper.GetRecurrenceDateTimeCollection(recurringShift.RecurrenceRule, recurringShift.StartTime);
                    foreach (var date in childShiftDates)
                    {
                        bool dateIsAvailable = recurringShift.Volunteer == null && date > DateTime.Now.Date.AddDays(1) && 
                            volunteer.Availabilities
                                .Any(a => recurringShift.StartTime.TimeOfDay >= a.StartTime && recurringShift.EndTime.TimeOfDay <= a.EndTime) &&
                                volunteer.Positions.Any(p => p.Id == recurringShift.PositionId);

                        if (dateIsAvailable)
                        {
                            var availableShift = new Shift()
                            {
                                StartTime = date + recurringShift.StartTime.TimeOfDay,
                                EndTime = date + recurringShift.EndTime.TimeOfDay,
                                Position = recurringShift.Position
                            };

                            availableShifts.Add(availableShift);
                        }
                    }
                }

                // order shifts by ascending date
                return availableShifts.OrderBy(s => s.StartTime).ToList();
            }
            return null;
        }
    }
}
