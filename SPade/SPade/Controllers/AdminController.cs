using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPade.Controllers
{
    public class AdminController : Controller
    {

        [Authorize(Roles = "")]
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult BulkAddLecturer()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult BulkAddStudent()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult AddOneStudent()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult AddOneClass()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult AddOneLecturer()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult ManageClass()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult ManageStudent()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult ManageLecturer()
        {
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult UpdateClass()
        {
            return View();
        }
        
        [Authorize(Roles = "")]
        public ActionResult UpdateStudent()
        {
            return View();
        }
        
        [Authorize(Roles = "")]
        public ActionResult UpdateLecturer()
        {
            return View();
        }
    }
}