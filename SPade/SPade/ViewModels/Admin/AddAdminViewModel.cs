using System.ComponentModel.DataAnnotations;

namespace SPade.ViewModels.Admin
{
    public class AddAdminViewModel
    {
        [Required]
        public string AdminID { get; set; }
        [Required]
        [RegularExpression("^[0-9]{8,8}$", ErrorMessage = "Please enter a proper Singapore-based phone number")]
        [Display(Name = "Contact Number")]
        public int ContactNo { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Do not exceed 50 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
    }
}