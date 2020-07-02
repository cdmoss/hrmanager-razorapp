using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Data.Models
{
    [Serializable]
    public class Reference
    {
        public int Id { get; set; }
        public VolunteerProfile Volunteer { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Relationship { get; set; }
        [Required]
        public string Occupation { get; set; }
    }
}
