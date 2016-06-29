using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.Models
{
    public class Lecturer
    {
        public string LecturerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public virtual ICollection<Lec_Class> Lec_Class { get; set; }
    }
}