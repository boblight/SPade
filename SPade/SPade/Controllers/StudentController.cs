using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Models;
using SPade.ViewModels;
using SPade.ViewModels.Student;
using System.Threading.Tasks;
using System.IO;

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

        //POST: SubmitAssignment
        [HttpPost]
        public Task<ActionResult> SubmitAssignment(SubmitAssignmentViewModel svm)
        {
            if (ModelState.IsValid)
            {
                // Use your file here
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    //  svm.File.InputStream.CopyTo(memoryStream);
                }
            }
            return null;
        }

        // GET: SubmitAssignment
        public ActionResult SubmitAssignment(int id)
        {
            List<Assignment> pass = new List<Assignment>();
            SubmitAssignmentViewModel svm = new SubmitAssignmentViewModel();
            Assignment ass = db.Assignments.ToList().Find(a => a.AssgnID == id);

            svm.AssgnID = ass.AssgnID;
            svm.Describe = ass.Describe;
            svm.DueDate = ass.DueDate;
            svm.CreateBy = ass.CreateBy;
            svm.ModuleCode = ass.ModuleCode;
            svm.MaxAttempt = ass.MaxAttempt;

            return View(svm);
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