using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace SPade.ViewModels.Admin
{
    public class AddStudentViewModel
    {
        [Required]
        [StringLength(8, ErrorMessage = "Please enter a valid Admin No.")]
        [RegularExpression("^[s]+$", ErrorMessage = "Please enter valid Admin No.")]
        public string AdminNo { get; set; }
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
        public List<Class> Classes { get; set; }
        public string ClassName { get; set; }
        public int ClassID { get; set; }
    }
}