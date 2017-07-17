using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.ViewModels.Student;

namespace SPade.Models
{
    [Serializable]
    public class SolutionScoreClass
    {
        public int solutionScoreKey { get; set; }
        public List<Result> descriptionScore { get; set; }
        public List<string> testcaseInput { get; set; }
        public List<string> testcaseDescription { get; set; }
    }
}