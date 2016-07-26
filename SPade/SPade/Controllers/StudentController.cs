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
using System.Text.RegularExpressions;

namespace SPade.Controllers
{
  //  [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private SPadeDBEntities db = new SPadeDBEntities();

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
            int assgnId = (int)Session["assignmentId"];
            Assignment assignment = db.Assignments.ToList().Find(a => a.AssignmentID == assgnId);
            //query for which programming language needed to be used for this assignment
            //for "Scalability sake"
            ProgLanguage langUsed = db.ProgLanguages.ToList().Find(p => p.LanguageId == db.Modules.ToList().Find(m => m.ModuleCode == assignment.ModuleCode).LanguageId);

            //getting file path
            //first check if file us empty of is not zip file
            if (file != null && Path.GetExtension(file.FileName) == ".zip")
            {
                if (file.ContentLength > 0)
                {
                    //save zip file in submissions directory temporarily
                    //next unzip it into the same folder while replacing existing submissions
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var zipLocation = Server.MapPath(@"~/Submissions/" + file);
                    file.SaveAs(zipLocation);

                    string title = Regex.Replace(assignment.AssgnTitle, @"\s+", "");
                    string submissionName = User.Identity.GetUserName() + title + assignment.AssignmentID;
                    var filePath = Server.MapPath(@"~/Submissions/" + submissionName + "/" + fileName.ToLower());
                    var filePathForGrade = Server.MapPath(@"~/Submissions/" + submissionName + "/");
                    System.IO.DirectoryInfo fileDirectory = new DirectoryInfo(filePath);

                    if (fileDirectory.Exists)
                    {
                        foreach (FileInfo files in fileDirectory.GetFiles())
                        {
                            files.Delete();//delete all files in directory
                        }
                        foreach (DirectoryInfo dir in fileDirectory.GetDirectories())
                        {
                            dir.Delete(true);
                        }
                    }
                    fileDirectory.Create(); // Recreates directory to update latest submission
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipLocation, filePath);

                    //grade submission
                    Grader grader = new Grader(filePathForGrade, fileName, assgnId, langUsed.LangageType);
                    decimal result = grader.grade();

                    submission.Grade = result;
                    submission.AssignmentID = assgnId;
                    submission.AdminNo = User.Identity.GetUserName();
                    submission.FilePath = submissionName;
                    submission.Timestamp = DateTime.Now;
                }
            }
            else if (file == null)
            {
                Session["UploadError"] = "Please select a file to upload.";
                return RedirectToAction("SubmitAssignment", assgnId);
            }
            else if (Path.GetExtension(file.FileName) != ".zip")
            {
                Session["UploadError"] = "Only zip files are allowed. Please zip up your project before uploading.";
                return RedirectToAction("SubmitAssignment", assgnId);
            }

            db.Submissions.Add(submission);
            db.SaveChanges();

            Session["submission"] = submission;

            return RedirectToAction("PostSubmission");
        }//end of submit assignment

        // GET: SubmitAssignment
        public ActionResult SubmitAssignment(int id)
        {
            //List<Assignment> pass = new List<Assignment>();
            SubmitAssignmentViewModel svm = new SubmitAssignmentViewModel();
            Assignment assignment = db.Assignments.ToList().Find(a => a.AssignmentID == id);
            svm.RetryRemaining = assignment.MaxAttempt - db.Submissions.ToList().FindAll(s => s.AssignmentID == id).Count();

            Module module = db.Modules.ToList().Find(m => m.ModuleCode == assignment.ModuleCode);
            svm.Module = module.ModuleCode + " " + module.ModuleName;

            //start a session to check which assignment student is viewing
            Session["assignmentId"] = id;

            if (Session["UploadError"] != null)
            {
                ModelState.AddModelError("UploadError", Session["UploadError"].ToString());
                Session["UploadError"] = null;
            }

            svm.assignment = assignment;

            return View(svm);
        }//end of get SubmitAssignment

        // GET: ViewAssignment
        public ActionResult ViewAssignment()
        {
            List<ViewAssignmentViewModel> vm = new List<ViewAssignmentViewModel>();
            List<Assignment> assignments = new List<Assignment>();

            //to replace hardcoded classid with sessions values
            List<Class_Assgn> ca = db.Class_Assgn.ToList().FindAll(c => c.ClassID == 1);

            foreach (Class_Assgn i in ca)
            {
                assignments = db.Assignments.ToList().FindAll(assgn => assgn.AssignmentID == i.AssignmentID);

                foreach (Assignment a in assignments)
                {
                    ViewAssignmentViewModel v = new ViewAssignmentViewModel();
                    v.RetryRemaining = a.MaxAttempt - db.Submissions.ToList().FindAll(s => s.AssignmentID == a.AssignmentID).Count();
                    v.assignment = a;

                    Module module = db.Modules.ToList().Find(m => m.ModuleCode == a.ModuleCode);
                    v.Module = module.ModuleCode + " " + module.ModuleName;

                    //check if the assignment has been attempted before
                    if (db.Submissions.ToList().FindAll(s => s.AdminNo == User.Identity.GetUserName() && s.AssignmentID == a.AssignmentID).Count() > 0) //hardcoded admin number to be replaced by session admin numer
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
        public ActionResult ViewResult()
        {

            ViewResultViewModel vrvm = new ViewResultViewModel();

            string loggedInStudent = User.Identity.GetUserName();


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
                Submission.Add("/Student/Download/?file=" + Regex.Replace(r.assgntitle, @"\s+", "") + r.assignmentid);

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
        public ActionResult PostSubmission()
        {
            Submission submission = (Submission)Session["submission"];
            return View(submission);
        }


        [HttpGet]
        public ActionResult Download(string file)
        {
            string path = "~/Submissions/" + User.Identity.GetUserName() + file; //temp
            string zipname = User.Identity.GetUserName() + file + ".zip"; //temp

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