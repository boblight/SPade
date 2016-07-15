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
using System.Data.SqlClient;
using Ionic.Zip;

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
                var zipLocation = Server.MapPath(@"~/Submissions/" + file);
                file.SaveAs(zipLocation);

                var filePath = Server.MapPath(@"~/Submissions/" + User.Identity.GetUserName() + assgnId + fileName);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.
                System.IO.Compression.ZipFile.ExtractToDirectory(zipLocation, filePath);
                //file.SaveAs(filePath);

                //grading parts
                Grader grader = new Grader(filePath, fileName, assgnId);
                //Decimal result = Decimal.Parse(grader.grade().ToString());
                Decimal result = grader.grade();

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

            ViewResultViewModel vrvm = new ViewResultViewModel();

            string loggedInStudent = "p1431476"; //temp 


            var results = db.Database.SqlQuery<DBres>("select s1.submissionid, s1.adminno, s1.assignmentid, a.assignmentid, a.assgntitle, a.startdate, a.duedate, s1.grade, s1.filepath, s1.timestamp from submission s1 inner join( select max(submissionid) submissionid, adminno, assignmentid, max(timestamp) timestamp from submission group by adminno, assignmentid ) s2 on s1.submissionid = s2.submissionid inner join( select * from assignment where deletedat is null ) a on s1.assignmentid = a.assignmentid where s1.adminno = @inStudent",
    new SqlParameter("@inStudent", loggedInStudent)).ToList();

            List<String> Assignment = new List<String>();
            List<String> AssignmentId = new List<String>();
            List<String> IssuedOn = new List<String>();
            List<String> DueDate = new List<String>();
            List<String> Result = new List<String>();
            List<String> Overall = new List<String>();
            List<String> SubmittedOn = new List<String>();
            List<String> Submission = new List<String>();

            foreach (var r in results)
            {
                Assignment.Add(r.assgntitle);
                IssuedOn.Add(r.startdate.ToString());
                DueDate.Add(r.duedate.ToString());
                Result.Add((int)Math.Round(r.grade * 100) + "%");

                if (r.grade >= 0.5M)
                    Overall.Add("Pass");
                else
                    Overall.Add("Fail");

                SubmittedOn.Add(r.timestamp.ToString());
                Submission.Add("/Student/Download/?file=" +r.assgntitle +r.assignmentid);

            }

            vrvm.Assignment = Assignment;
            vrvm.IssuedOn = IssuedOn;
            vrvm.DueDate = DueDate;
            vrvm.Result = Result;
            vrvm.Overall = Overall;
            vrvm.SubmittedOn = SubmittedOn;
            vrvm.Submission = Submission;

            return View(vrvm);
        }


        // GET: PostSubmission
        // [Authorize(Roles = "")]
        public ActionResult PostSubmission()
        {
            ////temporary arrangement for me to self validate output - TL
            //SubmissionViewModel svm = new SubmissionViewModel();
            //svm = (SubmissionViewModel)Session["Submission"];
            //svm.submission = (Submission)Session["submission"];
            return View(Session["submission"]);
        }


        [HttpGet]
        public ActionResult Download(string file)
        {

            string path = "~/Submissions/" + "p1431476"+ file; //temp
            string zipname = "p1431476" +file + ".zip"; //temp

            var memoryStream = new MemoryStream();
            using (var zip = new ZipFile())
            {
                zip.AddDirectory(Server.MapPath(path));
                zip.Save(memoryStream);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/zip", zipname);


        }

        private class DBres
        {
            public string assgntitle { get; set; }
            public int assignmentid { get; set; }
            public DateTime startdate { get; set; }
            public DateTime duedate { get; set; }
            public decimal grade { get; set; }
            public string filepath { get; set; }
            public DateTime timestamp { get; set; }
        }

    }//end of controller
}