namespace SPade.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SPadeModel : DbContext
    {
        public SPadeModel()
            : base("name=SPadeModel")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Class_Assgn> Class_Assgn { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Lec_Class> Lec_Class { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<ProgLanguage> ProgLanguages { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .Property(e => e.AdminID)
                .IsUnicode(false);

            modelBuilder.Entity<Admin>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Admin>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            modelBuilder.Entity<Admin>()
                .HasOptional(e => e.Login)
                .WithRequired(e => e.Admin);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.Describe)
                .IsUnicode(false);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.Solution)
                .IsUnicode(false);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.ModuleCode)
                .IsUnicode(false);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.CreateBy)
                .IsUnicode(false);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.DeletedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Assignment>()
                .Property(e => e.AssgnTitle)
                .IsUnicode(false);

            modelBuilder.Entity<Assignment>()
                .HasMany(e => e.Class_Assgn)
                .WithRequired(e => e.Assignment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Assignment>()
                .HasMany(e => e.Submissions)
                .WithRequired(e => e.Assignment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.ClassName)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.DeletedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Lec_Class)
                .WithRequired(e => e.Class)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Class_Assgn)
                .WithRequired(e => e.Class)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .Property(e => e.CourseName)
                .IsUnicode(false);

            modelBuilder.Entity<Course>()
                .Property(e => e.CourseAbbr)
                .IsUnicode(false);

            modelBuilder.Entity<Course>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Course>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Course>()
                .Property(e => e.DeletedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.Classes)
                .WithRequired(e => e.Course)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lec_Class>()
                .Property(e => e.StaffID)
                .IsUnicode(false);

            modelBuilder.Entity<Lecturer>()
                .Property(e => e.StaffID)
                .IsUnicode(false);

            modelBuilder.Entity<Lecturer>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Lecturer>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Lecturer>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Lecturer>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Lecturer>()
                .Property(e => e.DeletedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Lecturer>()
                .HasMany(e => e.Lec_Class)
                .WithRequired(e => e.Lecturer)
                .HasForeignKey(e => e.StaffID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lecturer>()
                .HasMany(e => e.Lec_Class1)
                .WithRequired(e => e.Lecturer1)
                .HasForeignKey(e => e.StaffID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.LoginID)
                .IsUnicode(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.Salt)
                .IsUnicode(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.HashedPassword)
                .IsFixedLength();

            modelBuilder.Entity<Module>()
                .Property(e => e.ModuleCode)
                .IsUnicode(false);

            modelBuilder.Entity<Module>()
                .Property(e => e.ModuleName)
                .IsUnicode(false);

            modelBuilder.Entity<Module>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Module>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Module>()
                .Property(e => e.DeletedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Module>()
                .HasMany(e => e.Assignments)
                .WithRequired(e => e.Module)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProgLanguage>()
                .Property(e => e.LangageType)
                .IsUnicode(false);

            modelBuilder.Entity<ProgLanguage>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<ProgLanguage>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<ProgLanguage>()
                .Property(e => e.DeletedBy)
                .IsUnicode(false);

            modelBuilder.Entity<ProgLanguage>()
                .HasMany(e => e.Modules)
                .WithRequired(e => e.ProgLanguage)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.RoleName)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Logins)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.AdminNo)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.DeletedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .HasOptional(e => e.Login)
                .WithRequired(e => e.Student);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.Submissions)
                .WithRequired(e => e.Student)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Submission>()
                .Property(e => e.AdminNo)
                .IsUnicode(false);

            modelBuilder.Entity<Submission>()
                .Property(e => e.Grade)
                .HasPrecision(3, 2);

            modelBuilder.Entity<Submission>()
                .Property(e => e.FilePath)
                .IsUnicode(false);
        }
    }
}
