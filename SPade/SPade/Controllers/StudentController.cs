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
using SPade.Grading;

namespace SPade.Controllers
{
    public class StudentController : Controller
    {
        private SPadeEntities db = new SPadeEntities();
        private Grader grader = new Grader();

        // GET: Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        //POST: SubmitAssignment
        [HttpPost]
        public async Task<ActionResult> SubmitAssignment(HttpPostedFileBase file)
        {
            Submission submission = new Submission();

            //getting file path
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Server.MapPath(@"~/App_Data/Submissions/" + "1431476" /*student id temp, to get from session*/ + "1" /*assignment id*/ + fileName);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.
                file.SaveAs(filePath);

                //grading hardcoded assignment id to be changed
                Decimal temp = Decimal.Parse(grader.grade("~/App_Data/Submissions/" + "1431476" /*student id temp, to get from session*/ + "1" /*assignment id*/ + fileName, 1).ToString());
                if (temp != 2)
                {
                    submission.Grade = temp;
                    submission.AssgnID = int.Parse(Session["id"].ToString());
                    submission.AdminNo = "1431476";
                    submission.FilePath = filePath.ToString();
                    submission.Timestamp = DateTime.Now;
                }
                else if (temp == 2)
                {
                    return Redirect("/Student/ViewAssignment");
                }
            }

            db.Submissions.Add(submission);

            Session["submission"] = submission;

            return RedirectToAction("PostSubmission");
        }

        // GET: SubmitAssignment
        public ActionResult SubmitAssignment(int id)
        {
            //start a session to check which assignment student is viewing
            Session["id"] = id;

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

        // GET: PostSubmission
        public ActionResult PostSubmission()
        {
            return View(Session["Submission"]);
        }
    }//end of controller
}