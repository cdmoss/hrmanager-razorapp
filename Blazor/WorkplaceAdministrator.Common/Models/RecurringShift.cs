using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WorkplaceAdministrator.Common
{
    public class RecurringShift : Shift
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
        public IList<Shift> ExcludedShifts { get; set; } = new List<Shift>();
        [NotMapped]
        public IList<Shift> ConstituentShifts
        {
            get
            {
                List<Shift> shifts = new List<Shift>();

                DateTime dateTracker = StartDate;

                while (dateTracker <= EndDate)
                {
                    if (NormalizedWeekdays.Contains(Enum.GetName(typeof(DayOfWeek), dateTracker.DayOfWeek).ToLower()))
                    {
                        shifts.Add(new Shift()
                        {
                            StartDate = dateTracker,
                            StartTime = StartTime,
                            EndTime = EndTime,
                            PositionWorked = PositionWorked
                        });
                    }
                    dateTracker = dateTracker.AddDays(1);
                }
                return shifts;
            }
        }

        public void UpdateRecurrenceRule()
        {
            DateTime combinedDateTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hours, StartTime.Minutes, StartTime.Seconds);

            string startDateStr = combinedDateTime.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);
            string endDateStr = EndDate.ToString("yyyyMMdd");
            string exDateStr = "";

            foreach (Shift shift in ExcludedShifts)
            {
                DateTime combined = new DateTime(shift.StartDate.Year, shift.StartDate.Month, shift.StartDate.Day, shift.StartTime.Hours, shift.StartTime.Minutes, shift.StartTime.Seconds);
                exDateStr += $"\\nEXDATE:{combined.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}Z";
            }

            RecurrenceRule = $"DTSTART:{startDateStr}\\nRRULE:FREQ=WEEKLY;INTERVAL=1;UNTIL={endDateStr};BYDAY={Weekdays}{exDateStr}";
        }
    }
}
