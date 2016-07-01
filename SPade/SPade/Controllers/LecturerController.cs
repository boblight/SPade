using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Models;

namespace SPade.Controllers
{
    public class LecturerController : Controller
    {
        //init the db
        private SPadeEntities db = new SPadeEntities();

        // GET: Lecturer
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ManageClassesAndStudents()
        {
            //get the data from the database 
            //we sort them according to the lecturer and what not 
            //store in a list and append it to the page 

            //things to note: how am i gonna settle the counting of students

            string x = "s123456"; //temp 

            List<Lec_Class> lecClassList = new List<Lec_Class>();
            List<Class> classList = new List<Class>();
            //List<Class> managedClasses = db.Class.Where(c => c.Lec_Class.Where(lc => lc.ClassID == c.ClassID).FirstOrDefault().LecturerID == x).ToList();
            List<Class> managedClasses = db.Classes.Where(c => c.CourseID == 1 && c.Course.CourseName == "DIT").ToList();

            lecClassList = db.Lec_Class.ToList();
            classList = db.Classes.ToList();

            //foreach (Lec_Class lc in lecClassList)
            //{
            //    if (lc.sta == x)//lecturer ID matches 
            //    {
            //        foreach (Class c in classList)
            //        {
            //            if (c.ClassID == lc.ClassID) //class ID matches the class ID of that row 
            //            {
            //                managedClasses.Add(c); //store the item inside the list
            //            }
            //        }
            //    }
            //}

            return View(managedClasses);

        }

        public ActionResult BulkAddStudent()
        {
            return View();
        }

        public ActionResult ViewStudentsByClass()
        {
            return View();
        }

        public ActionResult UpdateStudent()
        {
            return View();
        }

        public ActionResult ManageAssignments()
        {
            return View();
        }

        public ActionResult AddAssignment()
        {
            return View();
        }

        public ActionResult UpdateAssignment()
        {
            return View();
        }

        public ActionResult ViewResults()
        {
            return View();
        }

    }//end of controller
}