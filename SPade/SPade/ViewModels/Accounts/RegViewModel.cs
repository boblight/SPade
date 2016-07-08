using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models;

namespace SPade.ViewModels.Accounts
{
    public class RegViewModel : SPade.Models.Student
    {
        public List<Class> classList{ get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}