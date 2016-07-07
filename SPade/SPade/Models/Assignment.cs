namespace SPade.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Assignment")]
    public partial class Assignment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Assignment()
        {
            Class_Assgn = new HashSet<Class_Assgn>();
            Submissions = new HashSet<Submission>();
        }

        [Key]
        public int AssgnID { get; set; }

        [Required]
        [StringLength(5000)]
        public string Describe { get; set; }

        public int MaxAttempt { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        [StringLength(200)]
        public string Solution { get; set; }

        [Required]
        [StringLength(20)]
        public string ModuleCode { get; set; }

        public DateTime CreateAt { get; set; }

        [Required]
        [StringLength(20)]
        public string CreateBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        [StringLength(20)]
        public string DeletedBy { get; set; }

        [Required]
        [StringLength(100)]
        public string AssgnTitle { get; set; }

        public DateTime StartDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Class_Assgn> Class_Assgn { get; set; }

        public virtual Module Module { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
