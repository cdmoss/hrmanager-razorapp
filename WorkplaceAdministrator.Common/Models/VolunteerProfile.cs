using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkplaceAdministrator.Web.Data.Models
{
    [Serializable]
    public class VolunteerProfile
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "First name (Required)")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last name (Required)")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Address (Required)")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "City (Required)")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Postal code (Required)")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Postal code must match one of the following expressions: LNLNLN, LNL-NLN, LNL NLN.")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name = "Main phone (Required)")]
        [Phone]
        public string MainPhone { get; set; }
        [Display(Name = "Alternate phone 1 (Optional)")]
        [Phone]
        public string AlternatePhone1 { get; set; }
        [Display(Name = "Alternate phone 2 (Optional)")]
        [Phone]
        public string AlternatePhone2 { get; set; }
        [Required]
        [Display(Name= "Birth date (Required)")]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        [Required]
        [Display(Name = "Full name of emergency contact (Required)")]
        public string EmergencyFullName { get; set; }
        [Required]
        [Display(Name = "Emergency contact phone 1 (Required)")]
        [Phone]
        public string EmergencyPhone1 { get; set; }
        [Display(Name = "Emergency contact phone 2 (Optional)")]
        [Phone]
        public string EmergencyPhone2 { get; set; }
        [Required]
        [Display(Name = "Relationship to emergency contact (Required)")]
        public string EmergencyRelationship { get; set; }
        [Display(Name = "Food safe")]
        public bool FoodSafe { get; set; }
        [DataType(DataType.Date)]
        public DateTime FoodSafeExpiry { get; set; }

        [Display(Name = "First aid")]
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
        [Required]
        [Display(Name = "Skills, interests and hobbies (If none then write none)")]
        public string SkillsInterestsHobbies { get; set; }
        [Required]
        [Display(Name = "Previous volunteer experience (If none then write none)")]
        public string VolunteerExperience { get; set; }
        [Required]
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
        public IList<PositionVolunteer> Positions { get; set; }
        public IList<Alert> Alerts { get; set; }
        public int UserID { get; set; }
        public AppUser User { get; set; }

        [NotMapped]
        public string FullNameWithID { get { return Id + " " + FirstName + " " + LastName; } }
    }
}
