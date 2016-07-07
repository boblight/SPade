namespace SPade.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Lec_Class
    {
        [Required]
        [StringLength(20)]
        public string StaffID { get; set; }

        public int ClassID { get; set; }

        [Key]
        [Column("_id")]
        public int C_id { get; set; }

        public virtual Class Class { get; set; }

        public virtual Lecturer Lecturer { get; set; }

        public virtual Lecturer Lecturer1 { get; set; }
    }
}
