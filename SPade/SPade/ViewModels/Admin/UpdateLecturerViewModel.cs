using SPade.ViewModels.Lecturer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class UpdateLecturerViewModel
    {
        
        public string StaffID { get; set; }
        public string Name { get; set; }
        public int ContactNo { get; set; }
        public List<AssignmentClass> ClassList { get; set; }
        [Required(ErrorMessage = "Please select classes to assign to")]
        [Display(Name = "assigned classes")]
        public string SelectedClasses { get; set; }
    }
}