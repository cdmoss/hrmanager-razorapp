using System;
using System.Collections.Generic;
using System.Text;
using MHFoodBank.Web.Data.Models;

namespace MHFoodBank.Web.Dtos
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
        public DateTime EndDate { get; set; }
        public string Weekdays { get; set; }
        public string RecurrenceRule { get; set; }

        //public string NormalizedWeekdays
        //{
        //    get
        //    {
        //        string normalizedWeekdayStr = "";

        //        if (Weekdays.Contains("SU"))
        //        {
        //            normalizedWeekdayStr += "sunday";
        //        }
        //        if (Weekdays.Contains("MO"))
        //        {
        //            normalizedWeekdayStr += "monday";
        //        }
        //        if (Weekdays.Contains("TU"))
        //        {
        //            normalizedWeekdayStr += "tuesday";
        //        }
        //        if (Weekdays.Contains("WE"))
        //        {
        //            normalizedWeekdayStr += "wednesday";
        //        }
        //        if (Weekdays.Contains("TH"))
        //        {
        //            normalizedWeekdayStr += "thursday";
        //        }
        //        if (Weekdays.Contains("FR"))
        //        {
        //            normalizedWeekdayStr += "friday";
        //        }
        //        if (Weekdays.Contains("SA"))
        //        {
        //            normalizedWeekdayStr += "saturday";
        //        }

        //        return normalizedWeekdayStr;
        //    }
        //}
    }
}
