using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SPade.Models
{
    public class Class
    {
        public int ClassID { get; set; }
        public int CourseID { get; set; }
        public string ClassName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<Assgn_Class> Assgn_Class { get; set; }
        public virtual ICollection<Student> Student { get; set; }
        public virtual ICollection<Lec_Class> Lec_Class { get; set; }
    }
}