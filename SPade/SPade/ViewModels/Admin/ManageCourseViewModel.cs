using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class ManageCourseViewModel
    {
        public String CourseId { get; set; }
        public String CourseName { get; set; }
        public String Abbreviation { get; set; }
        public String CreatedBy { get; set; }
        public String ClassCount { get; set; }

    }
}