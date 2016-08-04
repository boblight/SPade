using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class ManageClassViewModel
    {
        public string ClassID { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }
        public string CreatedBy { get; set; }
        public string NumLecturers { get; set; }
        public string NumStudents { get; set; }

}
}