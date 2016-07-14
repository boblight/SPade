using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Student
{
    public class ViewResultViewModel
    {

        public List<String> Assignment { get; set; }
        public List<String> IssuedOn { get; set; }
        public List<String> DueDate { get; set; }
        public List<String> Result { get; set; }
        public List<String> Overall { get; set; }
        public List<String> SubmittedOn { get; set; }
        public List<String> Submission { get; set; }

    }
}