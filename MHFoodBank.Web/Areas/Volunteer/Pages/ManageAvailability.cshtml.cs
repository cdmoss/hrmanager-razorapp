using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Volunteer.Pages
{
    public class ManageAvailabilityModel : VolunteerPageModel
    {
        [BindProperty]
        public Dictionary<string, List<Availability>> OldAvailability { get; set; } = new Dictionary<string, List<Availability>>();
        [BindProperty]
        public string StatusMessage { get; set; }

        public ManageAvailabilityModel(FoodBankContext context, UserManager<AppUser> userManager) : base(userManager, context)
        {
        }

        public async Task<IActionResult> OnGet(string statusMessage = null)
        {
            StatusMessage = statusMessage;
            AppUser currentUser = _userManager.GetUserAsync(User).Result;
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(currentUser.VolunteerProfile).Collection(p => p.Availabilities).LoadAsync();
            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
            GetSortedAvailability(currentUser.VolunteerProfile.Availabilities);

            return Page();
        }

        public async Task<IActionResult> OnPostSaveTimesAsync(IFormCollection formData)
        {
            AppUser currentUser = await _userManager.GetUserAsync(User);
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(currentUser.VolunteerProfile).Collection(p => p.Availabilities).LoadAsync();
            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;

            List<Availability> oldAvailabilities = await _context.Availabilities.Where(a => a.Volunteer.Id == currentUser.VolunteerProfile.Id).ToListAsync();
            _context.Availabilities.RemoveRange(oldAvailabilities);

            await _context.SaveChangesAsync();

            List<Availability> newAvailabilites = GetAvailabilitiesFromFormData(formData, currentUser.VolunteerProfile);

            if(newAvailabilites != null)
            {
                await _context.Availabilities.AddRangeAsync(newAvailabilites);
            }
            else
            {
                await _context.Availabilities.AddRangeAsync(oldAvailabilities);
                await _context.SaveChangesAsync();
                return await OnGet("Error: One or more of the entered times are not valid.");
            }

            await _context.SaveChangesAsync();

            GetSortedAvailability(currentUser.VolunteerProfile.Availabilities);

            return RedirectToPage(new { statusMessage = "You successfully changed your availability." });
        }

        private void GetSortedAvailability(IList<Availability> availabilities)
        {
            string[] daysInWeek = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };

            foreach (string day in daysInWeek)
            {
                List<Availability> currentDayAvailbilities = availabilities.Where(a => a.AvailableDay == day).OrderBy(a => a.StartTime).ToList();
                OldAvailability.Add(day, currentDayAvailbilities);
            }
        }

        private List<Availability> GetAvailabilitiesFromFormData(IFormCollection formData, VolunteerProfile volunteer)
        {
            try
            {
                List<Availability> availabilities = new List<Availability>();
                string[] daysInWeek = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
                for (int i = 0; i < daysInWeek.Length; i++)
                {
                    string currentDay = daysInWeek[i];
                    int fieldCountForCurrentDay = formData.Keys.Count(k => k.Contains($"{currentDay}-1"));

                    for (int j = 1; j <= fieldCountForCurrentDay; j++)
                    {
                        string startTimeString = formData[$"{currentDay}-1-{j}"];
                        string endTimeString = formData[$"{currentDay}-2-{j}"];


                        if (string.IsNullOrWhiteSpace(startTimeString) ^ string.IsNullOrWhiteSpace(endTimeString))
                        {
                            ModelState.AddModelError("TimeError", "One or more of the entered times are not valid.");
                            return null;
                        }

                        if (string.IsNullOrWhiteSpace(startTimeString) || string.IsNullOrWhiteSpace(endTimeString))
                        {
                            continue;
                        }

                        string[] startTimeParts = startTimeString.Split(':');
                        string[] endTimeParts = endTimeString.Split(':');

                        int startHours = int.TryParse(startTimeParts[0], out int resultSH) ? resultSH : -1;
                        int startMinutes = int.TryParse(startTimeParts[1], out int resultSM) ? resultSM : -1;
                        int endHours = int.TryParse(endTimeParts[0], out int resultEH) ? resultEH : -1;
                        int endMinutes = int.TryParse(endTimeParts[1], out int resultEM) ? resultEM : -1;

                        if(startHours == -1 || startMinutes == -1 || endHours == -1 || endMinutes == -1)
                        {
                            ModelState.AddModelError("TimeError", "One or more of the entered times are not valid.");
                            return null;
                        }

                        TimeSpan startTime = new TimeSpan(int.Parse(startTimeParts[0]), int.Parse(startTimeParts[1]), 0);
                        TimeSpan endTime = new TimeSpan(int.Parse(endTimeParts[0]), int.Parse(endTimeParts[1]), 0);

                        Availability a = new Availability()
                        {
                            AvailableDay = daysInWeek[i],
                            StartTime = startTime,
                            EndTime = endTime,
                            Volunteer = volunteer 
                        };

                        availabilities.Add(a);
                    }
                }

                return availabilities;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}