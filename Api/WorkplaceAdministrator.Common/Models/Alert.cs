using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;

namespace WorkplaceAdministrator.Common
{
    public enum RequestStatus
    {
        Pending,
        Accepted,
        Declined
    }

    public abstract class Alert
    {
        public int Id { get; set; }
        public WorkplaceAccount User { get; set; }
        public DateTime Date { get; set; }
        public bool HasBeenRead { get; set; }

        protected virtual string GetDescription() 
        {
            return null;
        }
    }

    public class ApplicationAlert : Alert
    {
        protected override string GetDescription()
        {
            return $"Recieved a new application from {User.FirstName} {User.LastName}";
        }
    }

    public class ShiftRequestAlert : Alert
    {
        // necessary when oldshift is a recurring shift
        public Shift OldShift { get; set; }
        public Shift NewShift { get; set; }
        public string Reason { get; set; }
        public RequestStatus Status { get; set; }
        // the alert will persist in the database until both of the below properties == true
        public bool DismissedByAdmin { get; set; }
        public bool DismissedByVolunteer { get; set; }

        protected override string GetDescription()
        {
            // include information about the change they want to make?
            return $"{User.FirstName} {User.LastName} requested a shift change.";
        }
    }
}
