using Ionic.Zip;
using Microsoft.AspNet.Identity;
using SPade.Grading;
using SPade.Models.DAL;
using SPade.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;
using Hangfire;
using Newtonsoft.Json;

namespace SPade.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private SPadeDBEntities db = new SPadeDBEntities();
        private decimal Result;
        private string jobID = "";

        // GET: Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        //POST: SubmitAssignment
        [HttpPost]
        public ActionResult SubmitAssignment(HttpPostedFileBase file)
        {
            string submissionName = "";
            Submission tempSubmission = new Submission();
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
                    submissionName = User.Identity.GetUserName() + title + assignment.AssignmentID;
                    var filePath = Server.MapPath(@"~/Submissions/" + submissionName + "/" + fileName.ToLower());
                    var filePathForGrade = Server.MapPath(@"~/Submissions/" + submissionName);
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
                    //grade returns an 'exitcode'
                    //if result is more than 1 then is error code
                    //2 for execption thrown
                    //3 for infinite loop
                    //4 for run error
                    //anywhere from 0.0 - 1.0 determines the grade given to the particular submission

                    //this part is grading the assignment. we add to the scheduler to process it
                    var jobID = BackgroundJob.Enqueue(() => ProcessSubmission(filePathForGrade, fileName, assgnId, langUsed.LangageType));

                    //fill up the the submission model partially 
                    //only result not in -> this will come AFTER the scheduler successfully runs the assignment
                    tempSubmission.AssignmentID = assgnId;
                    tempSubmission.AdminNo = User.Identity.GetUserName();
                    tempSubmission.FilePath = submissionName;
                    tempSubmission.Timestamp = DateTime.Now;

                    Session["TempSub"] = tempSubmission;
                    Session["jobID"] = jobID;

                }
            }
            else if (file == null)
            {
                ModelState.AddModelError("UploadError", "Please select a file to upload.");
                return View();
            }
            else if (Path.GetExtension(file.FileName) != ".zip")
            {
                Session["UploadError"] = "Only zip files are allowed. Please zip up your project before uploading.";
                return RedirectToAction("SubmitAssignment", assgnId);
            }

            return RedirectToAction("PostSubmission");
        }//end of submit assignment

        public decimal ProcessSubmission(string filePathForGrade, string fileName, int assgnId, string langUsed)
        {
            decimal result;

            //the grading of the assignment is done here (the scheduler adds this to queue)
            Sandboxer sandBoxedGrading = new Sandboxer(filePathForGrade, fileName, assgnId, langUsed);
            result = sandBoxedGrading.runSandboxedGrading();

            return result;
        }

        public class DataObj
        {
            //this object is for the data that is retreived from the Hangfire State column
            public DateTime SucceededAt { get; set; }
            public int PerformanceDuration { get; set; }
            public int Latency { get; set; }
            public decimal Result { get; set; }
        }

        public bool QueryJobFinish(string jobId)
        {
            //this method is to check the DB IF the job is finished
            bool runningJob = true;
            int runningJobId = Int32.Parse(jobId);
            State succeededState = new State();

            //we check the Hangfire.State table to see if our current job has succeeded running 
            var stateList = db.States.Where(s => s.JobId == runningJobId).ToList();

            foreach (State s in stateList)
            {
                if (s.Name == "Succeeded")
                {
                    runningJob = false;
                    succeededState = s;
                }
            }

            //IF the job has been run, we get the result and add it togther with the earlier submission model we partially filled up
            if (runningJob == false)
            {
                //Hangfire stores the data as JSON. We deserialize it here to a DataObj -> which is shaped like JSON structure
                DataObj dObj = JsonConvert.DeserializeObject<DataObj>(succeededState.Data);
                //Retrieve the earlier submission model here
                Submission sub = (Submission)Session["TempSub"];

                sub.Grade = dObj.Result;

                db.Submissions.Add(sub);
                db.SaveChanges();
                Session.Remove("TempSub");
                //now we store the completed submission model in a session to use for the post assignment page
                Session["submission"] = sub;
            }

            //this is to indicate if the job has finished running or not
            return runningJob;
        }

        // GET: SubmitAssignment
        public ActionResult SubmitAssignment(int id)
        {
            //List<Assignment> pass = new List<Assignment>();
            SubmitAssignmentViewModel svm = new SubmitAssignmentViewModel();
            Assignment assignment = db.Assignments.ToList().Find(a => a.AssignmentID == id);
            svm.RetryRemaining = assignment.MaxAttempt - db.Submissions.ToList().FindAll(s => s.AssignmentID == id).Count();

            Module module = db.Modules.ToList().Find(m => m.ModuleCode == assignment.ModuleCode);
            svm.Module = module.ModuleCode + " " + module.ModuleName;
            svm.IssuedBy = db.Lecturers.ToList().Find(lc => lc.StaffID == assignment.CreateBy).Name.ToString();

            //start a session to check which assignment student is viewing
            Session["assignmentId"] = id;

            if (Session["UploadError"] != null)
            {
                ModelState.AddModelError("UploadError", Session["UploadError"].ToString());
                Session.Remove("UploadError");
            }

            svm.assignment = assignment;

            //encode the richtext from the DB 
            svm.assignment.Describe = AntiXssEncoder.HtmlEncode(svm.assignment.Describe, false);

            return View(svm);
        }//end of get SubmitAssignment

        // GET: ViewAssignment
        public ActionResult ViewAssignment()
        {
            List<ViewAssignmentViewModel> vm = new List<ViewAssignmentViewModel>();
            List<Assignment> assignments = new List<Assignment>();

            //to replace hardcoded classid with sessions values
            List<Class_Assgn> ca = db.Class_Assgn.ToList().FindAll(c => c.ClassID == db.Students.Where(stud => stud.AdminNo == User.Identity.Name).FirstOrDefault().ClassID);

            foreach (Class_Assgn i in ca)
            {
                assignments = db.Assignments.Where(a => a.DeletedAt == null).ToList().FindAll(assgn => assgn.AssignmentID == i.AssignmentID);

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
                {
                    Overall.Add("Pass");
                }
                else
                {
                    Overall.Add("Fail");
                }

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
            //this is to get the job running status
            bool isJobRunning;

            //we get the job code that is assigned to it
            string jobSessionId = (string)Session["jobID"];

            do
            {
                //check if the job has finished running or not
                isJobRunning = QueryJobFinish(jobSessionId);

            } while (isJobRunning);

            Submission submission = (Submission)Session["submission"];

            if (submission.Grade == 2)
            {
                ModelState.AddModelError("SubmissionError", "Your program has failed to run properly. This could be due to an exception being thrown " +
                    "or syntax error in your code. Please ensure your inputs are validated. Otherwise, check for logic/syntax error.");
                submission.Grade = 0;
            }
            else if (submission.Grade == 3)
            {
                ModelState.AddModelError("SubmissionError", "Your program has encountered an infinite loop. Please check through your program and make appropriate " +
                    "modification.");
                submission.Grade = 0;
            }
            else if (submission.Grade == 4)
            {
                ModelState.AddModelError("SubmissionError", "Error occured when attempting to run code. Please check through your code for syntax errors or missing parenthesis.");
                submission.Grade = 0;
            }
            else if (submission.Grade < 1)
            {
                ModelState.AddModelError("SubmissionError", "Program has failed a couple of test cases. Please check through your program and make appropriate " +
                    "modification to meet the requirements stated in the assignment description.");
            }

            Session.Remove("submission"); //clear session
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