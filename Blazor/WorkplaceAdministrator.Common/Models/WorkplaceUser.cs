using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text;

namespace WorkplaceAdministrator.Common
{
    public class WorkplaceAccount : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        /*
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string MainPhone { get; set; }
        public string AlternatePhone1 { get; set; }
        public string AlternatePhone2 { get; set; }
        public DateTime Birthdate { get; set; }
        public string EmergencyFullName { get; set; }
        public string EmergencyPhone1 { get; set; }
        public string EmergencyPhone2 { get; set; }
        public string EmergencyRelationship { get; set; }
        public bool FoodSafe { get; set; }
        [DataType(DataType.Date)]
        public DateTime FoodSafeExpiry { get; set; }
        public bool FirstAid { get; set; }
        [DataType(DataType.Date)]
        public DateTime FirstAidExpiry { get; set; }
        [Display(Name = "CPR")]
        public bool Cpr { get; set; }
        [DataType(DataType.Date)]
        public DateTime CprExpiry { get; set; }
        [Display(Name = "Other Certificates")]
        public string OtherCertificates { get; set; }
        [Required]
        [Display(Name = "Education and training (Required)")]
        public string EducationTraining { get; set; }
        [Display(Name = "Skills, interests and hobbies (If none then write none)")]
        public string SkillsInterestsHobbies { get; set; }
        [Display(Name = "Previous volunteer experience (If none then write none)")]
        public string VolunteerExperience { get; set; }
        [Display(Name = "Other boards you've appeared on (If none then write none)")]
        public string OtherBoards { get; set; }
        public bool VolunteerConfidentiality { get; set; }
        public bool VolunteerEthics { get; set; }
        public bool CriminalRecordCheck { get; set; }
        public bool DrivingAbstract { get; set; }
        public bool ConfirmationOfProfessionalDesignation { get; set; }
        public bool ChildWelfareCheck { get; set; }
        public bool OfficiallyApproved { get; set; }
        public IList<Reference> References { get; set; }
        public IList<WorkExperience> WorkExperiences { get; set; }
        public IList<Shift> Shifts { get; set; }
        public IList<Availability> Availabilities { get; set; }
        public IList<UserPosition> Positions { get; set; }
        public IList<Alert> Alerts { get; set; }
        */
    }
}
