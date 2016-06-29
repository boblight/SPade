using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.Models
{
    public class Student
    {
        public string StudentID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int ContactNo { get; set; }
        public int ClassID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }
}