using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common.Utils;

namespace WorkplaceAdministrator.Web.Data.Models
{
    [Serializable]
    public class WorkExperience
    {
        public int Id { get; set; }
        public VolunteerProfile Volunteer { get; set; }
        [Required]
        [Display(Name = "Employer Name")]
        public string EmployerName { get; set; }
        [Required]
        [Display(Name = "Employer Address")]
        public string EmployerAddress { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DateLessThan("EndDate", ErrorMessage = "Start date must be before end date.")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required]
        [Display(Name = "Employer Phone")]
        [Phone]
        public string EmployerPhone { get; set; }
        [Required]
        [Display(Name = "Contact Name")]
        public string ContactPerson { get; set; }
        [Required]
        [Display(Name = "Position Worked")]
        public string PositionWorked { get; set; }
        public bool CurrentJob { get; set; }
    }
}
