using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class ManageAdminViewModel
    {
        public String AdminId { get; set; }
        public String Name { get; set; }
        public String ContactNo { get; set; }
        public String Email { get; set; }
        public String CreatedBy { get; set; }
    }
}