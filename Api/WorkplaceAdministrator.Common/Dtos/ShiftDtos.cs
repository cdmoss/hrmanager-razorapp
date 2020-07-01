using System;
using System.Collections.Generic;
using System.Text;

namespace WorkplaceAdministrator.Common.Dtos
{
    public class ShiftCreateDto
    {
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Position PositionWorked { get; set; }
        public UserShiftDto User { get; set; }
    }

    public class ShiftReadEditDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Position PositionWorked { get; set; }
        public UserShiftDto User { get; set; }
        public string Description { get; set; }
    }
}
