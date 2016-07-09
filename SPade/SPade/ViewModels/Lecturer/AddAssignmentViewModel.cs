using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Lecturer
{
    public class AddAssignmentViewModel : Assignment
    {
        public List<AssignmentClass> ClassList { get; set; }
        public List<Module> Modules { get; set; }
        public string ModuleId { get; set; }
    }
}