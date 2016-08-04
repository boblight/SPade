using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class ManageLecturerViewModel
    {
        public string StaffID { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string CreatedBy { get; set; }
        public string NumClasses { get; set; }
    }
}