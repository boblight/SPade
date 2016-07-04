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

        private SPadeEntities db = new SPadeEntities();
        

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
        public ActionResult AddOneClass([Bind(Include = "CourseID, ClassName")]Class class1)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Classes.Add(class1);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(class1);
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