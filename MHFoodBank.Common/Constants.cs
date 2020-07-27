using System;
using System.Collections.Generic;
using System.Text;

namespace MHFoodBank.Common
{
    public class Constants
    {
        public class Regex
        {
            public const string postalCode = @"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$";
            public const string phone = @"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*";
            public const string password = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
        }
        public class RoleNames
        {
            public const string Admin = "Admin";
            public const string Staff = "Staff";
            public const string Volunteer = "Volunteer";
        }
        public class UserNames
        {
            public const string Admin = "fbadmin";
            public const string Staff = "staff";
            public const string Volunteer = "testvol";
        }
    }
}
