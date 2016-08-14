using SPade.Models.DAL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPade.ViewModels.Shared
{
    public class AddStudentViewModel
    {
        [Required]
        [StringLength(8, ErrorMessage = "Please enter a valid Admin No.")]
        [RegularExpression("^[p0-9]{8,8}$", ErrorMessage = "Please enter valid Admin No.")]
        public string AdminNo { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Do not exceed 50 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(50, ErrorMessage = "Email is too long.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^[0-9]{8,8}$", ErrorMessage = "Please enter a proper Singapore-based phone number")]
        [Display(Name = "Contact Number")]
        public int ContactNo { get; set; }
        public List<Class> Classes { get; set; }
        public List<string> className { get; set; }
        public List<int> classID { get; set; }
    }
}