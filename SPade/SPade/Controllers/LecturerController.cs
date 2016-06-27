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

        public ActionResult BulkAddStudent()
        {
            return View();
        }

        public ActionResult AddAssignment()
        {
            return View();
        }

        public ActionResult ManageAssignment()
        {
            return View();
        }

        public ActionResult ManageClassesAndStudent()
        {
            return View();
        }
    }
}