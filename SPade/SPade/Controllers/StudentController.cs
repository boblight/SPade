using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPade.Controllers
{
    public class StudentController : Controller
    {
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
            return View();
        }

        // GET: ViewResult
        public ActionResult ViewResult()
        {
            return View();
        }
    }//end of controller
}