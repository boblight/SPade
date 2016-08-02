using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class AddLecturerViewMode
    {
        [Required]
        [StringLength(8, ErrorMessage = "Please enter a valid Staff number")]
        [RegularExpression("^[s]+$", ErrorMessage = "Please enter valid Staff number")]
        public string StaffID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Do not exceed 50 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^[0-9]{8,8}$", ErrorMessage = "Please enter a proper Singapore-based phone number")]
        [Display(Name = "Contact Number")]
        public int ContactNo { get; set; }
    }
}