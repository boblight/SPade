using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class AddClassViewModel
    {

        public int ClassID { get; set; }
        public int CourseID { get; set; }
        public string ClassName { get; set; }

    }
}