using MHFoodBank.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MHFoodBank.Web.Dtos
{
    public class WorkExperienceDto
    {
        public int Id { get; set; }
        [Display(Name = "Employer Name")]
        public string EmployerName { get; set; }
        [Display(Name = "Employer Address")]
        public string EmployerAddress { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Employer Phone")]
        [Phone]
        public string EmployerPhone { get; set; }
        [Display(Name = "Contact Name")]
        public string ContactPerson { get; set; }
        [Display(Name = "Position Worked")]
        public string PositionWorked { get; set; }
        public bool CurrentJob { get; set; }
    }
}
