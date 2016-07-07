namespace SPade.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Course")]
    public partial class Course
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }

        [Required]
        [StringLength(200)]
        public string CourseName { get; set; }

        [Required]
        [StringLength(10)]
        public string CourseAbbr { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(20)]
        public string CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        [StringLength(20)]
        public string DeletedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Class> Classes { get; set; }
    }
}
