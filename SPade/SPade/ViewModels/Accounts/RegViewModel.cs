using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models;
using SPade.Models.DAL;

namespace SPade.ViewModels.Accounts
{
    public class RegViewModel : SPade.Models.DAL.Student
    {
        public List<Class> classList{ get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}