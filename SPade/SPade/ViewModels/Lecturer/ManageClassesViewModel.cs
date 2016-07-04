using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Lecturer
{
    public class ManageClassesViewModel
    {
        public int Id { get; set; }
        public String ClassName { get; set; }
        public int NumberOfStudents { get; set; }
    }
}