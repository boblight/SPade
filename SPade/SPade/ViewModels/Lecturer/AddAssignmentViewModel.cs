using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models;

namespace SPade.ViewModels.Lecturer
{
    public class AddAssignmentViewModel : Assignment
    {
        public List<Class> ClassList { get; set; }
        public List<Module> Modules { get; set; }

    }
}