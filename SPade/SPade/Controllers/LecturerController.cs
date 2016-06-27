using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPade.Controllers
{
    public class LecturerController : Controller
    {
        // GET: Lecturer
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ManageClassesAndStudents()
        {
            return View();

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