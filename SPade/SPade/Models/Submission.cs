using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.Models
{
    public class Submission
    {
        public int SubmissionID { get; set; }
        public string StudentID { get; set; }
        public int AssignmentID { get; set; }
        public string FilePath { get; set; }
        public DateTime Timestamp { get; set; }
    }
}