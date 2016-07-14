using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Models;
using SPade.Models.DAL;
using SPade.ViewModels;
using SPade.ViewModels.Student;
using System.Threading.Tasks;
using System.IO;
using SPade.Grading;
using System.Diagnostics;
using Microsoft.AspNet.Identity;

namespace SPade.Controllers
{
    public class StudentController : Controller
    {
        private SPadeDBEntities db = new SPadeDBEntities();

        // GET: Dashboard
        //[Authorize(Roles = "")]
        public ActionResult Dashboard()
        {
            return View();
        }
        
        //POST: SubmitAssignment
        [HttpPost]
        //[Authorize(Roles = "")]
        public async Task<ActionResult> SubmitAssignment(HttpPostedFileBase file)
        {
            Submission submission = new Submission();
            int assgnId = (int)Session["assignmentId"];
            Assignment assignment = db.Assignments.ToList().Find(a => a.AssignmentID == assgnId);

            //getting file path
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var zipLocation = Server.MapPath(@"~/App_Data/Submissions/" + file);
                file.SaveAs(zipLocation);

                var filePath = Server.MapPath(@"~/App_Data/Submissions/" + User.Identity.GetUserName() + assgnId + fileName);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.
                System.IO.Compression.ZipFile.ExtractToDirectory(zipLocation, filePath);
                //file.SaveAs(filePath);

                //grading parts
                Grader grader = new Grader(filePath, fileName, assgnId);
                Decimal result = Decimal.Parse(grader.grade().ToString());

                if (result != 2)
                {
                    submission.Grade = result;
                    submission.AssignmentID = assgnId;
                    //submission.AdminNo = User.Identity.GetUserName().ToString();
                    submission.AdminNo = User.Identity.GetUserName();
                    submission.FilePath = filePath.ToString();
                    submission.Timestamp = DateTime.Now;
                }
                else if (result == 2)
                {
                    //if grading encounters error
                    return Redirect("/Student/ViewAssignment"); //to implement proper handling soon
                }
            }

            db.Submissions.Add(submission);
            //assignment.MaxAttempt -= 1; //updating the max attempt
            db.SaveChanges();

            Session["submission"] = submission;

            return RedirectToAction("PostSubmission");
        }//end of submit assignment

        // GET: SubmitAssignment
        //[Authorize(Roles = "")]
        public ActionResult SubmitAssignment(int id)
        {
            List<Assignment> pass = new List<Assignment>();
            SubmitAssignmentViewModel svm = new SubmitAssignmentViewModel();
            Assignment assignment = db.Assignments.ToList().Find(a => a.AssignmentID == id);

            //start a session to check which assignment student is viewing
            Session["assignmentId"] = id;

            svm.assignment = assignment;

            return View(svm);
        }

        // GET: ViewAssignment
        //[Authorize(Roles = "")]
        public ActionResult ViewAssignment()
        {
            List<ViewAssignmentViewModel> vm = new List<ViewAssignmentViewModel>();
            List<Assignment> assignments = new List<Assignment>();

            //to replace hardcoded classid with sessions values
            List<Class_Assgn> ca = db.Class_Assgn.ToList().FindAll(c => c.ClassID == 1);

            foreach (Class_Assgn i in ca)
            { 
                assignments = db.Assignments.ToList().FindAll(assgn => assgn.AssignmentID == i.AssignmentID);

                foreach(Assignment a in assignments)
                {
                    ViewAssignmentViewModel v = new ViewAssignmentViewModel();
                    v.assignment = a;
                    //check if the assignment has been attempted before
                    if (db.Submissions.ToList().FindAll(s => s.AdminNo == "1431476" && s.AssignmentID == a.AssignmentID).Count() > 0) //hardcoded admin number to be replaced by session admin numer
                    {
                        v.timestamp = db.Submissions.ToList().Find(s => s.AssignmentID == a.AssignmentID).Timestamp;
                        v.submitted = true;
                    }
                    else
                    {
                        v.submitted = false;
                    }
                    vm.Add(v);
                }
            }
            return View(vm);
        }

        // GET: ViewResult
       // [Authorize(Roles = "")]
        public ActionResult ViewResult()
        {
            return View();
        }

        // GET: PostSubmission
       // [Authorize(Roles = "")]
        public ActionResult PostSubmission()
        {
            return View(Session["Submission"]);
        }
    }//end of controller
}