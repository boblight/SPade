using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models;
using SPade.Models.DAL;

namespace SPade.ViewModels.Lecturer
{
    public class ClassesAndModulesViewModel
    {
        //public String ModuleName { get; set; }
        // public int ModuleId { get; set; }
        public List<Class> ClassList { get; set; }

        public String[] ModuleName { get; set; }

        public List<Module> Modules { get; set; }
    }
}