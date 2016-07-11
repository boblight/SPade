using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;


namespace SPade.ViewModels.Admin
{
    public class AddStudentViewModel
    {

        public string AdminNo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ContactNo { get; set; }
        public List<Class> Classes { get; set; }
        public List<Lecturer> Lecturers { get; set; }
        public string ClassName { get; set; }
        public int ClassId { get; set; }


    }
}