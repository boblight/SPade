using SPade.Models.DAL;
using System.Collections.Generic;

namespace SPade.ViewModels.Admin
{
    public class PurgeViewModel
    {
        public List<Assignment> allAssignments { get; set; }
        public List<Submission> allSubmission { get; set; }
        public List<Class> allClasses { get; set; }
        public List<Class_Assgn> classAssgnRel { get; set; }
    }
}