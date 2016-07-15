using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models;
using SPade.Models.DAL;

namespace SPade.ViewModels.Student
{
    public class SubmitAssignmentViewModel
    {
        /*
        [Required, Microsoft.Web.Mvc.FileExtensions(Extensions = "csv",
             ErrorMessage = "Specify a CSV file. (Comma-separated values)")]*/
        public HttpPostedFile File { get; set; }

        public Assignment assignment { get; set; }

        public int RetryRemaining { get; set; }

    }//end of class
}