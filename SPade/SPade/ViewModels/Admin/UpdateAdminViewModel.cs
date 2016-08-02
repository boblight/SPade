using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.ViewModels.Admin
{
    public class UpdateAdminViewModel
    {

        public string AdminID { get; set; }
        public int ContactNo { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> DeletedAt { get; set; }
        public string DeletedBy { get; set; }




    }
}