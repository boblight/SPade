using SPade.ViewModels.Lecturer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Admin
{
    public class UpdateLecturerViewModel
    {

        [EmailAddress]
        [MaxLength(50, ErrorMessage = "Email is too long.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^[s0-9]{2,50}$", ErrorMessage = "Please enter valid Staff number")]
        public string StaffID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Do not exceed 50 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }
        [Required]
        [RegularExpression("^[0-9]{8,8}$", ErrorMessage = "Please enter a proper Singapore-based phone number")]
        [Display(Name = "Contact Number")]
        public int ContactNo { get; set; }
        public List<AssignmentClass> ClassList { get; set; }
        [Required(ErrorMessage = "Please select classes to assign to")]
        [Display(Name = "assigned classes")]
        public bool ModuleCoordinator { get; set; }
        public string ModuleCode { get; set; }
        public List<Module> ModuleList { get; set; }


        //Validation Purposes
        [Required(ErrorMessage = "Please select a choice")]
        public string ModuleCoordinatorChoice { get; set; }
        [Required(ErrorMessage = "Please select at least one class to assign")]
        public string ClassName { get; set; }
        [Required(ErrorMessage = "Please select a Module")]
        public string ModuleName { get; set; }
    }
}