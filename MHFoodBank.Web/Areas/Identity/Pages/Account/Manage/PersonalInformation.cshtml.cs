using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Identity.Pages.Account.Manage
{
    public class PersonalInformationModel : VolunteerPageModel
    {
        [BindProperty(SupportsGet = true)] 
        public PersonalInfoInputModel PersonalInfo  { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Position> Positions { get; set; }
        [BindProperty] public string StatusMessage { get; set; }

        public PersonalInformationModel(FoodBankContext context, UserManager<AppUser> userManager, string currentPage = "Personal Information") : base(userManager, context, currentPage)
        {
        }

        public async Task<IActionResult> OnGet(string statusMessage = null)
        {
            Positions = await _context.Positions.Where(p => p.Name != "All").ToListAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(currentUser.VolunteerProfile).Collection(p => p.Positions).LoadAsync();
            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
            PersonalInfo = new PersonalInfoInputModel()
            {
                Address = currentUser.VolunteerProfile.Address,
                City = currentUser.VolunteerProfile.City,
                PostalCode = currentUser.VolunteerProfile.PostalCode,
                Email = currentUser.Email,
                MainPhone = currentUser.VolunteerProfile.MainPhone,
                AlternatePhone1 = currentUser.VolunteerProfile.AlternatePhone1,
                AlternatePhone2 = currentUser.VolunteerProfile.AlternatePhone2,
                EmergencyName = currentUser.VolunteerProfile.EmergencyFullName,
                EmergencyRelationship = currentUser.VolunteerProfile.EmergencyRelationship,
                EmergencyPhone1 = currentUser.VolunteerProfile.EmergencyPhone1,
                EmergencyPhone2 = currentUser.VolunteerProfile.EmergencyPhone2,
                Positions = currentUser.VolunteerProfile.Positions.ToList(),
                FirstAidCpr = currentUser.VolunteerProfile.FirstAidCpr,
                FirstAidCprExpiry = currentUser.VolunteerProfile.FirstAidCprExpiry,
                FirstAidCprLevel = currentUser.VolunteerProfile.FirstAidCprLevel,
                FoodSafe = currentUser.VolunteerProfile.FoodSafe,
                FoodSafeExpiry = currentUser.VolunteerProfile.FoodSafeExpiry,
                OtherCertificates = currentUser.VolunteerProfile.OtherCertificates
            };
            StatusMessage = statusMessage;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if(!ModelState.IsValid)
            {
                return await OnGet(statusMessage: "Error: One or more of the fields was not filled in properly.");
            }
            Positions = await _context.Positions.Where(p => p.Name != "All" && !p.Deleted).ToListAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            _context.Update(currentUser);
            currentUser.VolunteerProfile.Address = PersonalInfo.Address;
            currentUser.VolunteerProfile.City = PersonalInfo.City;
            currentUser.VolunteerProfile.PostalCode = PersonalInfo.PostalCode;
            currentUser.Email = PersonalInfo.Email;
            currentUser.UserName = PersonalInfo.Email;
            currentUser.NormalizedUserName = PersonalInfo.Email.ToUpper();
            currentUser.NormalizedEmail = PersonalInfo.Email.ToUpper();
            currentUser.VolunteerProfile.MainPhone = PersonalInfo.MainPhone;
            currentUser.VolunteerProfile.AlternatePhone1 = PersonalInfo.AlternatePhone1;
            currentUser.VolunteerProfile.AlternatePhone2 = PersonalInfo.AlternatePhone2;
            currentUser.VolunteerProfile.EmergencyFullName = PersonalInfo.EmergencyName;
            currentUser.VolunteerProfile.EmergencyRelationship = PersonalInfo.EmergencyRelationship;
            currentUser.VolunteerProfile.EmergencyPhone1 = PersonalInfo.EmergencyPhone1;
            currentUser.VolunteerProfile.EmergencyPhone2 = PersonalInfo.EmergencyPhone2;
            currentUser.VolunteerProfile.Positions = PersonalInfo.Positions;
            FillPreferredPositions(currentUser.VolunteerProfile);
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
            [Required]
            public string Address { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            [RegularExpression(Constants.Regex.postalCode, ErrorMessage = "Postal code must match one of the following expressions: LNLNLN, LNL-NLN, LNL NLN.")]
            public string PostalCode { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [RegularExpression(Constants.Regex.phone, ErrorMessage = "Please enter a valid phone number.")]
            public string MainPhone { get; set; }
            [RegularExpression(Constants.Regex.phone, ErrorMessage = "Please enter a valid phone number.")]
            public string AlternatePhone1 { get; set; }
            [RegularExpression(Constants.Regex.phone, ErrorMessage = "Please enter a valid phone number.")]
            public string AlternatePhone2 { get; set; }
            [Required]
            public string EmergencyName { get; set; }
            [Required]
            public string EmergencyRelationship { get; set; }
            [Required]
            [RegularExpression(Constants.Regex.phone, ErrorMessage = "Please enter a valid phone number.")]
            public string EmergencyPhone1 { get; set; }
            [RegularExpression(Constants.Regex.phone, ErrorMessage = "Please enter a valid phone number.")]
            public string EmergencyPhone2 { get; set; }
            [Display(Name = "Food safe")]
            public bool FoodSafe { get; set; }
            [DataType(DataType.Date)]
            public DateTime? FoodSafeExpiry { get; set; }
            [Display(Name = "Level")]
            public string FirstAidCprLevel { get; set; }
            [Display(Name = "First aid")]
            public bool FirstAidCpr { get; set; }
            [DataType(DataType.Date)]
            public DateTime? FirstAidCprExpiry { get; set; }
            [Display(Name = "Other Certificates")]
            public string OtherCertificates { get; set; }
            public List<PositionVolunteer> Positions { get; set; }
        }
    }
}