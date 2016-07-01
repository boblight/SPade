using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Models;
using SPade.ViewModels;
using SPade.ViewModels.Student;

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
        public ActionResult SubmitAssignment(int? AssgnID)
        {
            List<Assignment> pass = new List<Assignment>();
            pass = db.Assignments.ToList().FindAll(a => a.AssgnID == AssgnID);
            return View();
        }

        // GET: ViewAssignment
        public ActionResult ViewAssignment()
        {
            List<Assignment> assignments = new List<Assignment>();

            //to replace hardcoded classid with sessions values
            List<Class_Assgn> ca = db.Class_Assgn.ToList().FindAll(c => c.ClassID == 1);

            foreach (Class_Assgn i in ca)
            {
                assignments = db.Assignments.ToList().FindAll(assgn => assgn.AssgnID == i.AssgnID);
            }
            
            return View(assignments);
        }

        // GET: ViewResult
        public ActionResult ViewResult()
        {
            return View();
        }
    }//end of controller
}