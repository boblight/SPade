using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.Models
{
    public class Assgn_Class
    {
        public int Assgn_ClassID { get; set; }
        public int AssignmentID { get; set; }
        public int ClassID { get; set; }

        public virtual Class classes { get;set;}
        public virtual Assignment assignment { get; set;}
    }
}