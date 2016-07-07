namespace SPade.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Login")]
    public partial class Login
    {
        [StringLength(20)]
        public string LoginID { get; set; }

        [Required]
        [StringLength(200)]
        public string Salt { get; set; }

        public int RoleID { get; set; }

        [Required]
        [MaxLength(32)]
        public byte[] HashedPassword { get; set; }

        public virtual Admin Admin { get; set; }

        public virtual Role Role { get; set; }

        public virtual Student Student { get; set; }
    }
}
