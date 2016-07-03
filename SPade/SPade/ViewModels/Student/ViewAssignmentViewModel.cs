using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Student
{
    public class ViewAssignmentViewModel
    {
        public string Description { get; set;}
        public string IssuedBy { get; set; }
        public DateTime DueDate { get; set; }
        public int AssgnID { get; set; }
    }//end of class
}