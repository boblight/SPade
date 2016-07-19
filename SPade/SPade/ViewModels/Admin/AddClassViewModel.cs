using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Admin
{
    public class AddClassViewModel
    {

        public int ClassID { get; set; }
        public int CourseID { get; set; }
        public string ClassName { get; set; }

        public List<Course> Courses { get; set; }
        public string CourseName { get; set; }

        public List<SPade.Models.DAL.Lecturer> Lecturers { get; set; }
        public string Name { get; set; }


        public string StaffID { get; set; }
        public int C_id { get; set; }


    }
}