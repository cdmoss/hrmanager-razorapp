﻿using MHFoodBank.Common;
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
                List<Shift> workableShifts = GetWorkableShiftsFromAvailabilites(volunteer.VolunteerProfile.Availabilities);
                bool noWorkableShifts = !workableShifts.Any();

                // skip the volunteer if they can't work any shifts
                if (noWorkableShifts)
                {
                    continue;
                }

                string workableShiftsStr = "";

                foreach (var shift in workableShifts)
                {
                    workableShiftsStr += ">   " + shift.StartDate.ToString("dddd, dd MMMM yyyy") + " - " + shift.StartTime + " until " + shift.EndTime + "\n";
                }

                await _emailSender.SendEmailAsync(volunteer.Email, "Available open shifts - MHFB", $"Hello { volunteer.VolunteerProfile.FirstName} { volunteer.VolunteerProfile.LastName},\n\n"
                 + $"According to your availability, you can volunteer for some currently open shifts:\n\n" +
                           workableShiftsStr + "\nIf you can attend any of these shifts, sign up for them on your <a href='https://volunteer.mhfoodbank.com'>online account</a> at or <a href='mailto:schedule@mhfoodbank.com'>email</a> us at schedule@mhfoodbank.com.\n\n" +
                           "Thanks again for volunteering at the Medicine Hat Food Bank.");

            }
        }

        public List<Shift> GetWorkableShiftsFromAvailabilites(IList<Availability> availabilities)
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
    }
}
