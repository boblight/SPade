using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class ManageClassViewModel
    {
        public List<string> ClassID { get; set; }
        public List<string> StaffID { get; set; }
        public int Count { get; set; }
        public List<string> Name { get; set; }
        public SPade.Models.DAL.Lecturer lecturers { get; set; }
        public SPade.Models.DAL.Class classes { get; set; }

        
    }
}