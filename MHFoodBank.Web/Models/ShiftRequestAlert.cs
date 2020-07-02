using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Data.Models
{
    [Serializable]
    public class ShiftRequestAlert : Alert
    {
        public enum RequestStatus
        {
            Pending,
            Accepted,
            Declined
        }

        // necessary when oldshift is a recurring shift
        public Shift OldShift { get; set; }
        public Shift NewShift { get; set; }
        public string Reason { get; set; }
        public RequestStatus Status { get; set; }
        // the alert will persist in the database until both of the below properties == true
        public bool DismissedByAdmin { get; set; }
        public bool DismissedByVolunteer { get; set; }
        public string AddressedBy { get; set; }

        protected override string GetDescription()
        {
            // include information about the change they want to make?
            return $"{Volunteer.FirstName} {Volunteer.LastName} requested a shift change.";
        }
    }
}
