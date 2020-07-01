using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WorkplaceAdministrator.Common.Dtos
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
        public IList<ReferenceDto> References { get; set; }
        public IList<WorkExperienceDto> WorkExperiences { get; set; }
        public IList<AvailabilityDto> Availabilities { get; set; }
        public IList<Position> Positions { get; set; }
    }

    public class AccountAdminReadEditDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
        public DateTime FoodSafeExpiry { get; set; }
        public bool FirstAid { get; set; }
        public DateTime FirstAidExpiry { get; set; }
        public bool Cpr { get; set; }
        public DateTime CprExpiry { get; set; }
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
        public IList<Position> Positions { get; set; }
    }



    public class AccountAdminListDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserShiftDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AccountReadEditDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string MainPhone { get; set; }
        public string AlternatePhone1 { get; set; }
        public string AlternatePhone2 { get; set; }
        public string EmergencyFullName { get; set; }
        public string EmergencyPhone1 { get; set; }
        public string EmergencyPhone2 { get; set; }
        public string EmergencyRelationship { get; set; }
        public IList<Position> Positions { get; set; }
    }
}
