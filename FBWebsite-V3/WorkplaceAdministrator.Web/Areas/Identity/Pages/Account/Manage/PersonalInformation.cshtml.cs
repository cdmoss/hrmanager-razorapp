using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Web.Areas.Volunteer.Pages.Shared;
using WorkplaceAdministrator.Web.Data;
using WorkplaceAdministrator.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WorkplaceAdministrator.Web.Areas.Identity.Pages.Account.Manage
{
    public class PersonalInformationModel : VolunteerPageModel
    {
        private readonly UserManager<AppUser> _userManager;
        [BindProperty(SupportsGet = true)] 
        public PersonalInfoInputModel PersonalInfo  { get; set; }
        [BindProperty(SupportsGet = true)] 
        public List<Position> Positions { get; set; }
        [BindProperty(SupportsGet = true)] public AppUser CurrentUser { get; set; }
        [BindProperty] public string StatusMessage { get; set; }

        public PersonalInformationModel(FoodBankContext context, UserManager<AppUser> userManager) : base(userManager, context)
        {
            _userManager = userManager;
        }

        public async Task OnGet(string statusMessage)
        {
            Positions = await _context.Positions.Where(p => p.Name != "All").ToListAsync();
            CurrentUser = await _userManager.GetUserAsync(User);
            await _context.Entry(CurrentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(CurrentUser.VolunteerProfile).Collection(p => p.Positions).LoadAsync();
            LoggedInUser = CurrentUser.VolunteerProfile.FirstName + " " + CurrentUser.VolunteerProfile.LastName;
            PersonalInfo = new PersonalInfoInputModel()
            {
                Address = CurrentUser.VolunteerProfile.Address,
                City = CurrentUser.VolunteerProfile.City,
                PostalCode = CurrentUser.VolunteerProfile.PostalCode,
                Email = CurrentUser.Email,
                BirthDate = CurrentUser.VolunteerProfile.Birthdate.ToString("yy-MM-dd"),
                MainPhone = CurrentUser.VolunteerProfile.MainPhone,
                AlternatePhone1 = CurrentUser.VolunteerProfile.AlternatePhone1,
                AlternatePhone2 = CurrentUser.VolunteerProfile.AlternatePhone2,
                EmergencyName = CurrentUser.VolunteerProfile.EmergencyFullName,
                EmergencyRelationship = CurrentUser.VolunteerProfile.EmergencyRelationship,
                EmergencyPhone1 = CurrentUser.VolunteerProfile.EmergencyPhone1,
                EmergencyPhone2 = CurrentUser.VolunteerProfile.EmergencyPhone2,
            };
            StatusMessage = statusMessage;
        }

        public async Task<IActionResult> OnPost()
        {
            Positions = await _context.Positions.Where(p => p.Name != "All").ToListAsync();
            CurrentUser = await _userManager.GetUserAsync(User);
            await _context.Entry(CurrentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            _context.Update(CurrentUser);
            CurrentUser.VolunteerProfile.Address = PersonalInfo.Address;
            CurrentUser.VolunteerProfile.City = PersonalInfo.City;
            CurrentUser.VolunteerProfile.PostalCode = PersonalInfo.PostalCode;
            CurrentUser.VolunteerProfile.Birthdate = Convert.ToDateTime(PersonalInfo.BirthDate);
            CurrentUser.Email = PersonalInfo.Email;
            CurrentUser.VolunteerProfile.MainPhone = PersonalInfo.MainPhone;
            CurrentUser.VolunteerProfile.AlternatePhone1 = PersonalInfo.AlternatePhone1;
            CurrentUser.VolunteerProfile.AlternatePhone2 = PersonalInfo.AlternatePhone2;
            CurrentUser.VolunteerProfile.EmergencyFullName = PersonalInfo.EmergencyName;
            CurrentUser.VolunteerProfile.EmergencyRelationship = PersonalInfo.EmergencyRelationship;
            CurrentUser.VolunteerProfile.EmergencyPhone1 = PersonalInfo.EmergencyPhone1;
            CurrentUser.VolunteerProfile.EmergencyPhone2 = PersonalInfo.EmergencyPhone2;
            FillPreferredPositions(CurrentUser.VolunteerProfile);
            await _context.SaveChangesAsync();
            return RedirectToPage(new { statusMessage = "You successfully saved your changes." });
        }

        private void FillPreferredPositions(VolunteerProfile volunteer)
        {
            // clear out all current preferences for this volunteer
            List<PositionVolunteer> oldPositions = _context.PositionVolunteers
                .Where(p => p.Volunteer == volunteer).ToList();
            _context.PositionVolunteers.RemoveRange(oldPositions);
            volunteer.Positions = new List<PositionVolunteer>();
            foreach (Position position in Positions)
            {
                var preferred = Request.Form["preferred-" + position.Name];
                var assigned = Request.Form["assigned-" + position.Name];

                PositionVolunteer posVol = new PositionVolunteer()
                {
                    Volunteer = volunteer,
                    Position = position,
                };

                // this means it was preferred, StringValues are weird
                if (preferred.Count > 0 && assigned.Count == 0)
                {
                    posVol.Association = PositionVolunteer.AssociationType.Preferred;
                    volunteer.Positions.Add(posVol);
                }
                // this means it was assigned
                else if (preferred.Count == 0 && assigned.Count > 0)
                {
                    posVol.Association = PositionVolunteer.AssociationType.Assigned;
                    volunteer.Positions.Add(posVol);
                }
                // this means it was both
                else if (preferred.Count > 0 && assigned.Count > 0)
                {
                    posVol.Association = PositionVolunteer.AssociationType.PreferredAndAssigned;
                    volunteer.Positions.Add(posVol);
                }
            }
        }

        public class PersonalInfoInputModel
        {
            public string Address { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public string BirthDate { get; set; }
            public string Email { get; set; }
            public string MainPhone { get; set; }
            public string AlternatePhone1 { get; set; }
            public string AlternatePhone2 { get; set; }
            public string EmergencyName { get; set; }
            public string EmergencyRelationship { get; set; }
            public string EmergencyPhone1 { get; set; }
            public string EmergencyPhone2 { get; set; }
        }
    }
}