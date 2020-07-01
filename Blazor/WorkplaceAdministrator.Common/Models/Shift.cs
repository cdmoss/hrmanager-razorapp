using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkplaceAdministrator.Common
{
    public class Shift
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Position PositionWorked { get; set; }
        public WorkplaceAccount Volunteer { get; set; }
        public RecurringShift ParentRecurringShift { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }

        public void CreateDescription()
        {
            if (Volunteer != null)
            {
                Description = " - " + EndTime.ToString(@"hh\:mm") + ": " + Volunteer.FirstName + " " + Volunteer.LastName;
            }
            else
            {
                Description = " - " + EndTime.ToString(@"hh\:mm") + ": " + "Open";
            }
        }
    }
}
