using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;
using System.Web.UI;

namespace SPade.ViewModels.Admin
{
    public class UpdateStudentViewModel
    {

        public string AdminNo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ContactNo { get; set; }
        public List<Class> Classes { get; set; }
        public string ClassName { get; set; }
        public int ClassID { get; set; }


    }
}