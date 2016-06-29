using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.Models
{
    public class Lec_Class
    {
        public int Lec_ClassID { get; set; }
        public string LecturerID { get; set; }
        public int ClassID { get; set; }

        public virtual Lecturer lecturer { get; set; }
        public virtual Class classses { get; set; }
    }
}