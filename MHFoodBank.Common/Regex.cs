using System;
using System.Collections.Generic;
using System.Text;

namespace MHFoodBank.Common
{
    public static class CustomRegex
    {
        public const string postalCode = @"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$";
        public const string phone = @"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*";
        public const string password = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
    }
}
