using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Models;

namespace SPade.Controllers
{
    public class StudentController : Controller
    {
        private SPadeEntities db = new SPadeEntities();

        // GET: Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        // GET: SubmitAssignment
        public ActionResult SubmitAssignment()
        {
            return View();
        }

        // GET: ViewAssignment
        public ActionResult ViewAssignment()
        {
            //temporary user student id as 1

            return View();
        }

        // GET: ViewResult
        public ActionResult ViewResult()
        {
            return View();
        }
    }//end of controller
}