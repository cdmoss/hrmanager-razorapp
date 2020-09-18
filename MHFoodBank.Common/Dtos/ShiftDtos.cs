using System;
using System.Collections.Generic;
using System.Text;
using MHFoodBank.Common;
using MHFoodBank.Common.Services;

namespace MHFoodBank.Common.Dtos
{
    public class ShiftReadEditDto
    {
        public int Id { get; set; }
        [DateLessThan("EndTime")]
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Position PositionWorked { get; set; }
        public VolunteerMinimalDto Volunteer { get; set; }
        public string Description { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public int? RecurrenceID { get; set; }
    }
}
