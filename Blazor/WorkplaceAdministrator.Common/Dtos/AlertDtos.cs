using System;
using System.Collections.Generic;
using System.Text;

namespace WorkplaceAdministrator.Common.Dtos
{
    public class AdminAlertListDto
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
        public DateTime Date { get; set; }
        public bool HasBeenRead { get; set; }
        public string AlertType { get; set; }
    }

    public class AdminArchivedAlertListDto
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
        public string AddressedBy { get; set; }
        public DateTime Date { get; set; }
        public string AlertType { get; set; }
    }

    public class UserShiftRequestListDto
    {
        public int Id { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime OriginalShiftDate { get; set; }
        public DateTime? RequestedShiftDate { get; set; }
        public RequestStatus Status { get; set; }
    }

    public class ShiftRequestReadDto
    {
        public int Id { get; set; }
        public ShiftReadEditDto OriginalShift { get; set; }
        public ShiftReadEditDto RequestedShift { get; set; }
        public string Reason { get; set; }
    }

    public class ShiftRequestCreateDto
    {
        public int OriginalShiftId { get; set; }
        public int RequestedShiftId { get; set; }
        public string Reason { get; set; }

    }
}
