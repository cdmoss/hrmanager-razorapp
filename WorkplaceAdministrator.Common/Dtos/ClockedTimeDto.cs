using System;
using System.Collections.Generic;
using System.Text;
using WorkplaceAdministrator.Web.Data.Models;

namespace WorkplaceAdministrator.Common.Dtos
{
    public class ClockedTimeDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Position Position { get; set; }
        public VolunteerMinimalDto Volunteer { get; set; }
    }
}
