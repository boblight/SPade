using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class ManageModuleViewModel
    {
        public String ModuleCode { get; set; }
        public String ModuleName { get; set; }
        public String ProgrammingLanguage { get; set; }
        public String CreatedBy { get; set; }
    }
}