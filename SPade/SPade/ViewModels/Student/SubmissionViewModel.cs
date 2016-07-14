using SPade.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Student
{
    public class SubmissionViewModel
    {
        public int AssignmentID { get; set; }

        public List<String> SubmissionOutput { get; set; }

        public List<String> SolutionOutput { get; set; }

        public Submission submission { get; set; }
    }//end of class
}