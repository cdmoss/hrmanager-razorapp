using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Common
{
    [Serializable]
    // the shift class encompasses three kinds of shifts:
    // - a regular shift
    // - a recurring shift
    // - a shift that was changed from a recurring set
    public class Shift
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Position PositionWorked { get; set; }
        public VolunteerProfile Volunteer { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public int? RecurrenceID { get; set; }

        public void CreateDescription()
        {
            if (Volunteer != null)
            {
                Description = Volunteer.FirstName + " " + Volunteer.LastName + " - " + PositionWorked.Name;
            }
            else
            {
                Description = "Open" + " - " + PositionWorked.Name;
            }
        }
    }
}
