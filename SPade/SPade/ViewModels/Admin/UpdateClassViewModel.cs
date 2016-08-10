using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Admin
{
    public class UpdateClassViewModel
    {
        internal string StaffID;

        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public List<Course> Courses { get; set; }
    }
}