using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SPade.ViewModels.Lecturer
{
    public class AddAssignmentViewModel
    {
        [Required(ErrorMessage = "Please provide a valid assignment title")]
        [StringLength(100, ErrorMessage = "Please keep the title below 100 characters", MinimumLength = 1)]
        [RegularExpression("^[A-z0-9 ]+$", ErrorMessage = "Please enter only alphanumeric characters.")]
        [Display(Name = "Assignment Title")]
        public string AssgnTitle { get; set; }

        public string ModuleId { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Please provide a valid question")]
        [Display(Name = "Assignment Question")]
        public string Describe { get; set; }

        [AllowHtml]
        public string Hints { get; set; }

        [Required]
        public string Solution { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please select a due date!")]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Max Attempts")]
        public int MaxAttempt { get; set; }

        [Required(ErrorMessage = "Please select classes to assign to")]
        [Display(Name = "assigned classes")]
        public string SelectedClasses { get; set; }
        public bool IsTestCasePresent { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public List<AssignmentClass> ClassList { get; set; }
        public List<Module> Modules { get; set; }
        public int IsPostBack { get; set; }
        public bool IsHintsPresent { get; set; }
    }
}