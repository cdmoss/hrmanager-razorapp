using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MHFoodBank.Common.Dtos
{
    public class ReferenceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [RegularExpression(@"\D*([2-9]\d{2})(\D*)([2-9]\d{2})(\D*)(\d{4})\D*", ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }
        public string Relationship { get; set; }
        public string Occupation { get; set; }
    }
}
