using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Student
{
    [Serializable]
    public class Result
    {
        public string Description { get; set; }
        public int Score { get; set; }
    }
}