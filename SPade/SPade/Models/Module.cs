namespace SPade.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Module")]
    public partial class Module
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Module()
        {
            Assignments = new HashSet<Assignment>();
        }

        [Key]
        [StringLength(20)]
        public string ModuleCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ModuleName { get; set; }

        public int LanguageId { get; set; }

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
        public virtual ICollection<Assignment> Assignments { get; set; }

        public virtual ProgLanguage ProgLanguage { get; set; }
    }
}
