using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Student
{
    public class submissionSolution
    {
        public List<Result> Solution { get; set; }
        public Submission Submission { get; set; }
        public string Hints { get; set; }
    }
}