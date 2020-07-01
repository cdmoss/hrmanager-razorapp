using System;
using System.Collections.Generic;
using System.Text;

namespace WorkplaceAdministrator.Common.Dtos
{
    public class WorkExperienceDto
    {
        public int Id { get; set; }
        public string EmployerName { get; set; }
        public string EmployerAddress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EmployerPhone { get; set; }
        public string ContactPerson { get; set; }
        public string PositionWorked { get; set; }
    }
}
