using System;
using System.Collections.Generic;
using System.Text;
using WorkplaceAdministrator.Web.Data.Models;

namespace WorkplaceAdministrator.Common.Dtos
{
    public class ShiftReadEditDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Position PositionWorked { get; set; }
        public VolunteerMinimalDto Volunteer { get; set; }
        public RecurringShiftReadEditDto ParentRecurringShift { get; set; }
        public string Description { get; set; }
    }

    public class RecurringShiftReadEditDto : ShiftReadEditDto
    {
        // need to add start time and end time to work with workable shift email
        public DateTime EndDate { get; set; }
        public string Weekdays { get; set; }

        public string NormalizedWeekdays
        {
            get
            {
                string normalizedWeekdayStr = "";

                if (Weekdays.Contains("SU"))
                {
                    normalizedWeekdayStr += "sunday";
                }
                if (Weekdays.Contains("MO"))
                {
                    normalizedWeekdayStr += "monday";
                }
                if (Weekdays.Contains("TU"))
                {
                    normalizedWeekdayStr += "tuesday";
                }
                if (Weekdays.Contains("WE"))
                {
                    normalizedWeekdayStr += "wednesday";
                }
                if (Weekdays.Contains("TH"))
                {
                    normalizedWeekdayStr += "thursday";
                }
                if (Weekdays.Contains("FR"))
                {
                    normalizedWeekdayStr += "friday";
                }
                if (Weekdays.Contains("SA"))
                {
                    normalizedWeekdayStr += "saturday";
                }

                return normalizedWeekdayStr;
            }
        }
        public string RecurrenceRule { get; set; }
    }
}
