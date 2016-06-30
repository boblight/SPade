using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.Models;

namespace SPade.DAL
{
    public class SpadeDbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<SpadeContext>
    {

        protected override void Seed(SpadeContext context)
        {
            var testClass = new List<Class>
            {
                new Class {ClassID=1, ClassName="DIT/1A/22"},
                new Class {ClassID=2, ClassName="DIT/1A/23"},
                new Class {ClassID=3, ClassName="DIT/1B/22"},
                new Class {ClassID=4, ClassName="DIT/1C/22"}
            };

            var testLec = new List<Lecturer>
            {
                new Lecturer {LecturerID="s123456", Name="LJK"},
                new Lecturer {LecturerID="s123457", Name="THS" },
                new Lecturer {LecturerID="s222222", Name="LKK" }
            };

            var testLC = new List<Lec_Class>
            {
                 new Lec_Class {Lec_ClassID=1, LecturerID="s123456", ClassID=1},
                 new Lec_Class {Lec_ClassID=2, LecturerID="s123456", ClassID=3},
                 new Lec_Class {Lec_ClassID=3, LecturerID="s222222", ClassID=2 }
            };

            testClass.ForEach(c => context.Class.Add(c));
            context.SaveChanges();

            testLec.ForEach(l => context.Lecturer.Add(l));
            context.SaveChanges();

            testLC.ForEach(f => context.Lec_Class.Add(f));

        }
    }//end of class
}