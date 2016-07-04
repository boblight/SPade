using SPade.Models;
using SPade.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPade.Controllers
{
    public class AdminController : Controller
    {

        private SPadeEntities2 db = new SPadeEntities2();
        

        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult BulkAddLecturer()
        {
            return View();
        }

        public ActionResult BulkAddStudent()
        {
            return View();
        }

        public ActionResult AddOneStudent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOneClass()
        {
            return View();
        }

        public ActionResult AddOneLecturer()
        {
            return View();
        }

        public ActionResult ManageClass()
        {
            return View();
        }

        public ActionResult ManageStudent()
        {
            return View();
        }
        public ActionResult ManageLecturer()
        {
            return View();
        }
        public ActionResult UpdateClass()
        {
            return View();
        }
        public ActionResult UpdateStudent()
        {
            return View();
        }
        public ActionResult UpdateLecturer()
        {
            return View();
        }






    }
}