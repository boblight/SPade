using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Student
{
    public class submissionSolution
    {
        public List<Result> solution { get; set; }
        public Submission submission { get; set; }
    }
}