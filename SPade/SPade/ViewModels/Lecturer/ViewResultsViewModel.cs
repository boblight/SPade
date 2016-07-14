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
        public List<String> classIds { get; set; }
        public List<String> classNames { get; set; }

    }
}