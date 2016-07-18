using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Lecturer
{
    public class BulkAddStudentViewModel
    {
        public string AdminNo { get; internal set; }
        public int ContactNo { get; internal set; }
        public string Email { get; internal set; }
        public string Name { get; internal set; }
    }
}