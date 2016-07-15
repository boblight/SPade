using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace SPade.ViewModels.Lecturer
{
    public class AddAssignmentViewModel
    {
        [Required]
        [Display(Name = "Assignment Title")]
        public string AssgnTitle { get; set; }
        public string ModuleId { get; set; }

        [Required]
        [Display(Name = "Assignment Question")]
        public string Describe { get; set; }

        [Required]
        public string Solution { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name ="Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name ="Max Attempts")]
        public int MaxAttempt { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public List<AssignmentClass> ClassList { get; set; }
        public List<Module> Modules { get; set; }

    }
}