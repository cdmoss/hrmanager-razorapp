using System;
using System.Collections.Generic;
using System.Text;

namespace MHFoodBank.Common
{ 
    public enum ApprovalStatus
    {
        Approved,
        Pending,
        NotApproved,
        Archived
    }

    public enum UserRole
    {
        Admin,
        Staff,
        Volunteer
    }
}
