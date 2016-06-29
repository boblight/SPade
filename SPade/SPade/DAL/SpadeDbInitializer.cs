using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPade.DAL
{
    public class SpadeDbInitializer : System.Data.Entity. DropCreateDatabaseIfModelChanges<SpadeContext>
    {
        protected override void Seed(SpadeContext context)
        {
            
        }
    }//end of class
}