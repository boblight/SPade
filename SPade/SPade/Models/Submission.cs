namespace SPade.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Submission")]
    public partial class Submission
    {
        public int SubmissionID { get; set; }

        [Required]
        [StringLength(20)]
        public string AdminNo { get; set; }

        public int AssgnID { get; set; }

        public decimal Grade { get; set; }

        [Required]
        [StringLength(200)]
        public string FilePath { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual Assignment Assignment { get; set; }

        public virtual Student Student { get; set; }
    }
}
