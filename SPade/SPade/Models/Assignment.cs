using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.Models
{
    public class Assignment
    {
        public int AssignmentID { get; set; }
        public string Description { get; set; }
        public int MaxAttempt { get; set; }
        public DateTime DueDate { get; set; }
        public string Solution { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public virtual ICollection<Assgn_Class> Assgn_Class { get; set; }
    }
}