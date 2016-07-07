namespace SPade.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Lecturer")]
    public partial class Lecturer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lecturer()
        {
            Lec_Class = new HashSet<Lec_Class>();
            Lec_Class1 = new HashSet<Lec_Class>();
        }

        [Key]
        [StringLength(20)]
        public string StaffID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        public int ContactNo { get; set; }

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
        public virtual ICollection<Lec_Class> Lec_Class1 { get; set; }
    }
}
