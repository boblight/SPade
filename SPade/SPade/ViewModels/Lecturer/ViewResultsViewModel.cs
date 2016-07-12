using SPade.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.ViewModels.Lecturer;

namespace SPade.ViewModels.Lecturer
{
    public class ViewResultsViewModel
    {
        //public List<Object> results { get; set; }
        public List<String> classIds { get; set; }
        public List<String> classNames { get; set; }

        public List<String> assIds { get; set; }
        public List<String> assNames { get; set; }

        public List<String> subIds { get; set; }
        public List<String> admNos { get; set; }
        public List<String> names { get; set; }
        public List<String> assignmentIds { get; set; }
        public List<String> grades { get; set; }
        public List<String> solutions { get; set; }

    }
}