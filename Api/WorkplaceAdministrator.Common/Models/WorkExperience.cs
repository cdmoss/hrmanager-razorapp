using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WorkplaceAdministrator.Common.Utils;

namespace WorkplaceAdministrator.Common
{
    public class WorkExperience
    {
        public int Id { get; set; }
        public WorkplaceAccount User { get; set; }
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
    }
}
