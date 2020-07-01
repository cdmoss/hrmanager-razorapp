using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WorkplaceAdministrator.Common
{
    public class Reference
    {
        public int Id { get; set; }
        public WorkplaceAccount User { get; set; }
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
