using SPade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Student
{
    public class ViewAssignmentViewModel
    {
        public bool submitted { get; set; }

        public Assignment assignment { get; set; }

        public DateTime timestamp { get; set; }
    }//end of class
}