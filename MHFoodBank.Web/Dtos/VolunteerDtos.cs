using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using MHFoodBank.Common;
using MHFoodBank.Web.Services;

namespace MHFoodBank.Web.Dtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email (Required)")]
        [Compare("ConfirmEmail", ErrorMessage = "The email and confirmation email do not match.")]
        public string Email { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email (Required)")]
        public string ConfirmEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Your password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Your password must contain at least one letter, one number and one special character (@$!%*#?&)")]
        [Compare("ConfirmPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
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
        [AgeOver14(ErrorMessage = "You must be over 14 years of age.")]
        [Display(Name = "Birth date (Required)")]
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
        public DateTime? FoodSafeExpiry { get; set; }
        [Display(Name = "First aid")]
        public bool FirstAid { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FirstAidExpiry { get; set; }
        [Display(Name = "CPR")]
        public bool Cpr { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CprExpiry { get; set; }
        [Display(Name = "Other Certificates")]
        public string OtherCertificates { get; set; }
        [Display(Name = "Education and training")]
        public string EducationTraining { get; set; }
        [Display(Name = "Skills, interests and hobbies")]
        public string SkillsInterestsHobbies { get; set; }
        [Display(Name = "Previous volunteer experience")]
        public string VolunteerExperience { get; set; }
        [Display(Name = "Other boards you've appeared on")]
        public string OtherBoards { get; set; }
        public bool VolunteerConfidentiality { get; set; }
        public bool VolunteerEthics { get; set; }
        public bool CriminalRecordCheck { get; set; }
        public bool DrivingAbstract { get; set; }
        public bool ConfirmationOfProfessionalDesignation { get; set; }
        public bool ChildWelfareCheck { get; set; }
        public bool OfficiallyApproved { get; set; }
        public IList<ReferenceDto> References { get; set; }
        public IList<WorkExperienceDto> WorkExperiences { get; set; }
        public IList<AvailabilityDto> Availabilities { get; set; }
        public IList<Position> Positions { get; set; }
    }

    public class VolunteerAdminReadEditDto
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "first name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "last name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "city")]
        public string City { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Postal code must match one of the following expressions: LNLNLN, LNL-NLN, LNL NLN.")]
        [Display(Name = "postal code")]
        public string PostalCode { get; set; }
        [Required]
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "phone number")]
        public string MainPhone { get; set; }
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        public string AlternatePhone1 { get; set; }
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        public string AlternatePhone2 { get; set; }
        [Required]
        [Display(Name = "birth date")]
        [AgeOver14(ErrorMessage = "The volunteer must be over 14 years of age.")]
        public DateTime Birthdate { get; set; }
        [Required]
        [Display(Name = "emergency full name")]
        public string EmergencyFullName { get; set; }
        [Required]
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "emergency phone number")]
        public string EmergencyPhone1 { get; set; }
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        public string EmergencyPhone2 { get; set; }
        [Required]
        [Display(Name = "relationship with this emergency contact")]
        public string EmergencyRelationship { get; set; }
        public bool FoodSafe { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FoodSafeExpiry { get; set; }
        public bool FirstAid { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FirstAidExpiry { get; set; }
        public bool Cpr { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CprExpiry { get; set; }
        public string OtherCertificates { get; set; }
        public string EducationTraining { get; set; }
        public string SkillsInterestsHobbies { get; set; }
        public string VolunteerExperience { get; set; }
        public string OtherBoards { get; set; }
        public bool VolunteerConfidentiality { get; set; }
        public bool VolunteerEthics { get; set; }
        public bool CriminalRecordCheck { get; set; }
        public bool DrivingAbstract { get; set; }
        public bool ConfirmationOfProfessionalDesignation { get; set; }
        public bool ChildWelfareCheck { get; set; }
        public bool OfficiallyApproved { get; set; }
        public IList<ReferenceDto> References { get; set; }
        public IList<WorkExperienceDto> WorkExperiences { get; set; }
        public IList<AvailabilityDto> Availabilities { get; set; }
        public IList<PositionVolunteer> Positions { get; set; }
    }

    public class VolunteerMinimalDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullNameWithID { get { return Id + " " + FirstName + " " + LastName; } }
    }

    public class VolunteerReadEditDto
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Postal code must match one of the following expressions: LNLNLN, LNL-NLN, LNL NLN.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Main Phone Number")]
        public string MainPhone { get; set; }
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Alternate Phone Number")]
        public string AlternatePhone1 { get; set; }
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Alternate Phone Number")]
        public string AlternatePhone2 { get; set; }
        [Required]
        [Display(Name = "Emergency Contact Full Name")]
        public string EmergencyFullName { get; set; }
        [Required]
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Emergency Contact Phone Number")]
        public string EmergencyPhone1 { get; set; }
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Emergency Contact Phone Number")]
        public string EmergencyPhone2 { get; set; }
        [Required]
        [Display(Name = "Emergency Contact Relationship")]
        public string EmergencyRelationship { get; set; }
        public IList<Position> Positions { get; set; }
    }
}
