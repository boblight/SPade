using SPade.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SPade.DAL
{
    public class SpadeContext :DbContext
    {
        public SpadeContext() : base("SpadeContext")
        {

        }

        public DbSet<Admin> Admin { get; set; }
        public DbSet<Assgn_Class> Assgn_Class { get; set; }
        public DbSet<Assignment> Assignment { get; set; }
        public DbSet<Class> Class { get; set;}
        public DbSet<Course> Course { get; set; }
        public DbSet<Lec_Class> Lec_Class { get; set; }
        public DbSet<Lecturer> Lecturer { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Submission> Submission { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //prevent created table from being pluralized when creating tables from models
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }//end of class
}