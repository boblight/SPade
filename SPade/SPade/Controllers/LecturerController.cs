using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Models;
using SPade.ViewModels.Admin;
using SPade.ViewModels.Lecturer;
using SPade.ViewModels.Student;

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
            List<ManageClassesViewModel> manageClassView = new List<ManageClassesViewModel>();
            ManageClassesViewModel e = new ManageClassesViewModel();

            string x = "1431489"; //temp 

            //get the classes managed by the lecturer 
            List<Class> managedClasses = db.Classes.Where(c => c.Lec_Class.Where(lc => lc.ClassID == c.ClassID).FirstOrDefault().StaffID == x).ToList();

            //get the students in that classs
            foreach (Class c in managedClasses)
            {
                //match the class ID of student wit hthe class ID of the managed Classes
                var count = db.Students.Where(s => s.ClassID == c.ClassID).Count();

                e.ClassName = c.ClassName;
                e.Id = c.ClassID;
                e.NumberOfStudents = count;

                manageClassView.Add(e);

            }

            return View(manageClassView);

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

        //[HttpPost]
        //public ActionResult AddAssignment()
        //{



        //    return View();
        //}

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