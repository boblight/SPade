using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models.DAL;

namespace SPade.ViewModels.Lecturer
{
    public class ManageAssignmentViewModel
    {
        public Assignment Assignment { get; set; }
        public string Classes { get; set; }
    }
}