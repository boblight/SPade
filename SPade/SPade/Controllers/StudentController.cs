using Ionic.Zip;
using Microsoft.AspNet.Identity;
using SPade.Grading;
using SPade.Models;
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
        private static Dictionary<decimal,List<Result>> descriptionScore = new Dictionary<decimal, List<Result>>();

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
            var classId = db.Students.Single(s => s.AdminNo == User.Identity.Name).ClassID;

            //query for which programming language needed to be used for this assignment
            //for "Scalability sake"
            ProgLanguage langUsed = db.ProgLanguages.ToList().Find(p => p.LanguageId == db.Modules.ToList().Find(m => m.ModuleCode == assignment.ModuleCode).LanguageId);

            //getting file path
            //first check if file us empty of is not zip file
            if (file != null)
            {
                if (file.ContentLength > 0)
                {


                    //Get file extension to set file path
                    var fileExtensionName = Path.GetExtension(file.FileName);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);




                    //File format would be:  
                    // "assignmentId/classId/studentadmissionNumber/lowerCaseFileName/lowerCasefile.py or .java or  .net
                    string title = Regex.Replace(assignment.AssgnTitle, @"\s+", "");
                    submissionName = @""+ assignment.AssignmentID + "/" + classId + "/" + User.Identity.GetUserName();
                    var filePath = Server.MapPath(@"~/Submissions/" + assignment.AssignmentID + "/" + classId + "/" + User.Identity.GetUserName() +  "/" + fileName.ToLower() + "/" + fileName.ToLower() + fileExtensionName);
                    var filePathForGrade = Server.MapPath(@"~/Submissions/" + submissionName);
                    System.IO.DirectoryInfo fileDirectory = new DirectoryInfo(filePathForGrade);

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


                    //Create the file and save it under the given directory file path
                    //E.g    tempsubmission/sampleqns1/sampleqns1.java
                    var fileInfo = new FileInfo(filePath);
                    fileInfo.Directory.Create();
                    file.SaveAs(filePath);




                    //grade submission
                    //grade returns an 'exitcode'
                    //if result is more than 1 then is error code
                    //2 for execption thrown
                    //3 for infinite loop
                    //4 for run error
                    //5 for compilation error
                    //anywhere from 0.0 - 1.0 determines the grade given to the particular submission

                    //this part is grading the assignment. we add to the scheduler to process it
                    var jobID = BackgroundJob.Enqueue(() => ProcessSubmission(filePathForGrade, fileName, assgnId, langUsed.LangageType,User.Identity.GetUserName()));

                    //fill up the the submission model partially 
                    //only result not in -> this will come AFTER the scheduler successfully runs the assignment
                    tempSubmission.AssignmentID = assgnId;
                    tempSubmission.AdminNo = User.Identity.GetUserName();
                    tempSubmission.FilePath = submissionName + "/" + fileName.ToLower();
                    tempSubmission.Timestamp = DateTime.Now;

                    Session["TempSub"] = tempSubmission;
                    Session["jobID"] = jobID;
                }
            }
            else if (file == null)
            {
                ModelState.AddModelError("UploadError", "Please select a file to upload.");
                //return View();
                return RedirectToAction("SubmitAssignment", assgnId);
            }
            

            return RedirectToAction("PostSubmission");
        }//end of submit assignment



        [AutomaticRetry(Attempts = 1)]
        //the method which we call the scheduler to run
        public int ProcessSubmission(string filePathForGrade, string fileName, int assgnId, string langUsed,string userName)
        {
            var descriptionScoreKey = Int32.Parse(userName.Substring(1)+""+assgnId);

            //the grading of the assignment is done here (the scheduler adds this to queue)
            Sandboxer sandBoxedGrading = new Sandboxer(filePathForGrade, fileName, assgnId, langUsed,descriptionScoreKey);
            int result = (int)sandBoxedGrading.runSandboxedGrading();

            
            //in the event if result returns a decimal that represents an error
            if (sandBoxedGrading.descriptionScore.Count != 0)
            {
                if (descriptionScore.ContainsKey(result))
                {
                    descriptionScore.Remove(result);
                }
                descriptionScore.Add(result, sandBoxedGrading.descriptionScore);
                
            }
            return result;
        }

        //Check the DB if a job has finished running
        public bool QueryJobFinish(string jobId)
        {
            //this method is to check the DB IF the job is finished
            bool runningJob = true;
            decimal totalScore = 0;
            decimal finalScore = 0;
            int runningJobId = Int32.Parse(jobId);
            //this is the row with all the necessary data we need
            State finalState = new State();
            List<Result> scores = new List<Result>();

            //we check the Hangfire.State table to see if our current job has succeeded running 
            var stateList = db.States.Where(s => s.JobId == runningJobId).ToList();

            //why stateList must be 3 then it will find the data
            //Hangfire stores 3 job states -> Enqueued, Processing, Succeeded/Failure (found in Hangfire.State in DB)
            //the return value is stored in the last state, which is either succeeded or failed
            //to reduce workload on the server when querying job status, thats why it only goes into the loop when it has 'completed' the job
            if (stateList.Count() == 3)
            {
                //3 means job has finished running. now is to check the results 
                runningJob = false;
                finalState = stateList.Last();
                Submission sub = (Submission)Session["TempSub"]; //Retrieve the earlier submission model here

                if (finalState.Name == "Succeeded")
                {

                    //Hangfire stores the data as JSON. We deserialize it here to a DataObj -> which is shaped like JSON structure
                    HangfireData dataObj = JsonConvert.DeserializeObject<HangfireData>(finalState.Data);

                    //Get the dictionary key to access the related object 
                    var descriptionScoreKey = dataObj.Result;
                    if (descriptionScore.TryGetValue(descriptionScoreKey, out scores))
                    {
                        Session["TempResults"] = scores;
                        Session["TempSolutionKey"] = descriptionScoreKey;
                        foreach (var eachScore in scores)
                        {
                            totalScore += eachScore.Score;
                        }
                        finalScore = totalScore / scores.Count;
                    }
                    else
                    {
                        finalScore = descriptionScoreKey > 100 ? 0 : descriptionScoreKey;
                    }






                    sub.Grade = finalScore;
                    db.Submissions.Add(sub);
                    db.SaveChanges();
                }
                else
                {
                    //job finished but failed to run the solution
                    sub.Grade = 2;
                    db.Submissions.Add(sub);
                    db.SaveChanges();
                }

                Session.Remove("TempSub");

                //store the completed submission obj for use in the post submission page
                Session["submission"] = sub;

            }

            //this is to indicate if the job has finished running or not
            return runningJob;
        }

        // GET: SubmitAssignment
        public ActionResult SubmitAssignment(int id)
        {

            // student can only check those assignments that they are allowed to see
            // student cannot see other courses assignments


            //Use id to check if there is an assignment associated with this user
            var viewableAssignment = false;
            var associatedAssgn = db.Class_Assgn.Where(oneClass => oneClass.ClassID == db.Students.FirstOrDefault(stud => stud.AdminNo == User.Identity.Name).ClassID && oneClass.AssignmentID == id);
            var today = DateTime.Today.Date;
            if (associatedAssgn.Any())
            {
                var classAssgn = associatedAssgn.Single();
                var oneAssignment = db.Assignments.Single(assgn => assgn.AssignmentID == classAssgn.AssignmentID);
                if (today >= oneAssignment.StartDate.Date && today <= oneAssignment.DueDate.Date)
                {
                    viewableAssignment = true;
                }
            }



            if (viewableAssignment != true)
            {
                return RedirectToAction("ViewAssignment", "Student");
            }




            /*
             -------------------------ORIGINAL CODE-------------------------
             */

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
                    var today = DateTime.Today.Date;


                    //Check if student is able to look at the assignment
                    //prior to the start date and due date
                    if (today >= a.StartDate.Date && today <= a.DueDate.Date)
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
                //Submission.Add("/Student/Download/?file=" + Regex.Replace(r.assgntitle, @"\s+", "") + r.assignmentid);
                Submission.Add("/Student/Download/?file=" + r.filepath );
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

            //this is to count for any infinite loop
            int counter = 0;

            //we get the job code that is assigned to it
            string jobSessionId = (string)Session["jobID"];

            do
            {
                //check if the job has finished running or not
                isJobRunning = QueryJobFinish(jobSessionId);
                counter++;
            } while (isJobRunning && counter < 10000);

            Submission submission = (Submission)Session["submission"];

            if (counter >= 10000) //ran into infinite loop
            {
                //cancel job 
                BackgroundJob.Delete(jobSessionId);
                //set if run into infinite loop
                submission.Grade = 3;
            }

            Submission toUpdate = db.Submissions.Where(s => s.SubmissionID == submission.SubmissionID).FirstOrDefault();

            if (submission.Grade == 2)
            {
                ModelState.AddModelError("SubmissionError", "Your program has failed to run properly. This could be due to an exception being thrown " +
                                                            "or syntax error in your code. Please ensure your inputs are validated. Otherwise, check for logic/syntax error and make sure " +
                                                            "you have uploaded the correct program.");
                toUpdate.Grade = 0;
            }
            else if (submission.Grade == 3)
            {
                ModelState.AddModelError("SubmissionError", "Your program has encountered an infinite loop. Please check through your program and make appropriate " +
                                                            "modification.");
                toUpdate.Grade = 0;
            }
            else if (submission.Grade == 4)
            {
                ModelState.AddModelError("SubmissionError", "Error occured when attempting to run code. Please check through your code for syntax errors or missing parenthesis." +
                                                            "Also ensure that you have submitted the correct source code.");
                toUpdate.Grade = 0;
            }
            else if (submission.Grade == 5)
            {
                ModelState.AddModelError("SubmissionError", "Program cannot be compiled, ensure you submit the correct source code.");
                toUpdate.Grade = 0;
            }
            else if (submission.Grade < 1)
            {
                ModelState.AddModelError("SubmissionError", "Program has failed a couple of test cases. Please check through your program and make appropriate " +
                                                            "modification to meet the requirements stated in the assignment description.");
            }
            db.SaveChanges();

            
            submissionSolution submissions = new submissionSolution();
            submissions.submission = submission;
            submissions.solution = (List<Result>)Session["TempResults"] ?? new List<Result>();

            Session.Remove("submission"); //clear session
            if (submissions.solution == null)
            {
                if (descriptionScore.Count > 0)
                {
                    descriptionScore.Remove((int) Session["TempSolutionKey"]);
                }
                Session.Remove("TempResults");
                Session.Remove("TempSolutionKey");
            }
            

            return View(submissions);
        }

        [HttpGet]
        public ActionResult Download(string file)
        {
            var path = "~/Submissions/" + file; //temp
            var fileName = file.Split('/');
            var zipname = User.Identity.GetUserName() + "(" + fileName[fileName.Length - 1] + ").zip"; //temp

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