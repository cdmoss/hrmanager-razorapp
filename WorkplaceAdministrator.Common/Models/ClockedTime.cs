using System;
using System.Collections.Generic;
using System.Text;
using WorkplaceAdministrator.Web.Data.Models;

namespace WorkplaceAdministrator.Common.Models
{
    public class ClockedTime
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Position Position { get; set; }
        public VolunteerProfile VolunteerProfile { get; set; }
    }
}
