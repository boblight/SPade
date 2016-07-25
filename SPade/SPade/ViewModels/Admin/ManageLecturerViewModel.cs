using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class ManageLecturerViewModel
    {
        public List<string> StaffID { get; set; }
        public List<string> Name { get; set; }
        public List<string> ContactNo { get; set; }
        public List<string> Email { get; set; }
    }
}