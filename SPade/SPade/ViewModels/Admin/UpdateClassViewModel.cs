using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Admin
{
    public class UpdateClassViewModel
    {
        public int ClassID { get; set; }
        public int CourseID { get; set; }
        public string ClassName { get; set; }

        public List<Course> Courses { get; set; }
        public string CourseName { get; set; }

        public List<SPade.Models.DAL.Lecturer> Lecturers { get; set; }
        public string LecturerName { get; set; }
        public int LecturerID { get; set; }
    }
}