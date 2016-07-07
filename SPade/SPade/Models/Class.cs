namespace SPade.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Class")]
    public partial class Class
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Class()
        {
            Lec_Class = new HashSet<Lec_Class>();
            Class_Assgn = new HashSet<Class_Assgn>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClassID { get; set; }

        public int CourseID { get; set; }

        [Required]
        [StringLength(10)]
        public string ClassName { get; set; }

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
        public virtual ICollection<Lec_Class> Lec_Class { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Class_Assgn> Class_Assgn { get; set; }

        public virtual Course Course { get; set; }
    }
}
