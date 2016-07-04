using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models;

namespace SPade.ViewModels.Student
{
    public class SubmitAssignmentViewModel : Assignment
    {
        /*
        [Required, Microsoft.Web.Mvc.FileExtensions(Extensions = "csv",
             ErrorMessage = "Specify a CSV file. (Comma-separated values)")]*/
        public string FilePath { get; set; }

    }//end of class
}