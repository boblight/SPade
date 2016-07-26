using Ionic.Zip;
using Microsoft.AspNet.Identity;
using SPade.Grading;
using SPade.Models.DAL;
using SPade.ViewModels.Lecturer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPade.Controllers
{

    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        //init the db
        private SPadeDBEntities db = new SPadeDBEntities();

        // GET: Lecturer
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ManageClassesAndStudents()
        {
            List<ManageClassesViewModel> manageClassView = new List<ManageClassesViewModel>();
            ManageClassesViewModel e = new ManageClassesViewModel();

            string x = User.Identity.GetUserName(); //temp 

            //get the classes managed by the lecturer 
            List<Class> managedClasses = db.Classes.Where(c => c.Lec_Class.Where(lc => lc.ClassID == c.ClassID).FirstOrDefault().StaffID == x).ToList();

            //get the students in that classs
            foreach (Class c in managedClasses)
            {
                //match the class ID of student wit hthe class ID of the managed Classes
                var count = db.Students.Where(s => s.ClassID == c.ClassID).Count();

                Course cs = db.Courses.Where(cx => cx.CourseID == c.CourseID).First();

                e.ClassName = cs.CourseAbbr + "/" + c.ClassName;
                e.Id = c.ClassID;
                e.NumberOfStudents = count;

                manageClassView.Add(e);

            }

            return View(manageClassView);

        }

        public ActionResult BulkAddStudent()
        {
            return View();
        }

        public ActionResult ViewStudentsByClass(string classID)
        {
            return View();
        }

        public ActionResult UpdateStudent()
        {
            return View();
        }

        public ActionResult ManageAssignments()
        {
            List<ManageAssignmentViewModel> manageAssgn = new List<ManageAssignmentViewModel>();
            List<Assignment> lecAssgn = new List<Assignment>();
            //ManageAssignmentViewModel mmvm = new ManageAssignmentViewModel();

            //to store the classes assigned that assignment 
            List<string> classAssgn = new List<string>();

            //string lecturerID = "s1431489"; //temp 
            var lecturerID = User.Identity.GetUserId();

            //get the assignments that this lecturer created
            //lecAssgn = db.Assignments.Where(a => a.CreateBy == lecturerID && a.DeletedBy == null).ToList();
            lecAssgn = db.Assignments.ToList().FindAll(a => a.CreateBy == lecturerID && a.DeletedBy == null);

            //get the name of the classes assigned to an assignment 
            foreach (Assignment a in lecAssgn)
            {
                ManageAssignmentViewModel mmvm = new ManageAssignmentViewModel();

                //get all the classes that are assigned the particular assignment under this lecturer + the classes they manage only ! 
                var query = from c in db.Classes
                            join ca in db.Class_Assgn on c.ClassID equals ca.ClassID
                            where ca.AssignmentID.Equals(a.AssignmentID)
                            join cl in db.Lec_Class on c.ClassID equals cl.ClassID
                            where cl.StaffID.Equals(lecturerID)
                            select c.ClassName;

                classAssgn = query.ToList();

                var o = classAssgn.Last();
                string jc = "";

                foreach (string s in classAssgn)
                {
                    if (s.Equals(o))
                    {
                        jc += s;
                    }
                    else
                    {
                        jc += s + ",";
                    }
                }

                mmvm.Assignment = a;
                mmvm.classList = classAssgn;
                mmvm.Classes = jc;
                manageAssgn.Add(mmvm);
            }

            return View(manageAssgn);
        }

        public FileResult DownloadTestCase()
        {
            string f = Server.MapPath(@"~/TestCase/testcase.xml");
            byte[] fileBytes = System.IO.File.ReadAllBytes(f);
            string fileName = "testcase.xml";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult AddAssignment()
        {
            List<AssignmentClass> ac = new List<AssignmentClass>();
            AddAssignmentViewModel aaVM = new AddAssignmentViewModel();

            //  string x = "s1431489"; //temp 
            var x = User.Identity.GetUserName();

            //get the classes managed by the lecturer 
            List<Class> managedClasses = db.Classes.Where(c => c.Lec_Class.Where(lc => lc.ClassID == c.ClassID).FirstOrDefault().StaffID == x).ToList();

            //get the modules 
            List<Module> allModules = db.Modules.ToList();

            //we loop through the classList to fill up the assignmentclass -> which is used to populate 
            foreach (var c in managedClasses)
            {
                AssignmentClass a = new AssignmentClass();
                a.ClassName = c.ClassName;
                a.ClassId = c.ClassID;
                a.isSelected = false;
                ac.Add(a);
            }

            aaVM.ClassList = ac;
            aaVM.Modules = allModules;

            return View(aaVM);
        }

        [HttpPost]
        public ActionResult AddAssignment(AddAssignmentViewModel addAssgn, HttpPostedFileBase solutionsFileUpload, HttpPostedFileBase testCaseUpload)
        {
            //run solution with testcase
            if (addAssgn.IsTestCasePresent == true)
            {
                if ((solutionsFileUpload != null && Path.GetExtension(solutionsFileUpload.FileName) == ".zip") && (testCaseUpload != null && Path.GetExtension(testCaseUpload.FileName) == ".xml"))
                {
                    if (solutionsFileUpload.ContentLength > 0 && testCaseUpload.ContentLength > 0)
                    {
                        //SubmitWithTestCase(addAssgn, solutionsFileUpload, testCaseUpload);
                        string slnFilePath = "";
                        string slnName = (addAssgn.AssgnTitle).Replace(" ", "");
                        string actualFileName = "";

                        //save the solution
                        var zipLocation = Server.MapPath(@"~/TempSubmissions/" + solutionsFileUpload);
                        solutionsFileUpload.SaveAs(zipLocation);
                        slnFilePath = Server.MapPath(@"~/TempSubmissions/" + slnName);
                        DirectoryInfo fileDirectory = new DirectoryInfo(slnFilePath);
                        if (fileDirectory.Exists)
                        {
                            foreach (FileInfo files in fileDirectory.GetFiles())
                            {
                                files.Delete();
                            }
                            foreach (DirectoryInfo dir in fileDirectory.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                        }
                        fileDirectory.Create();
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipLocation, slnFilePath);

                        //get the actual folder name containing the submission
                        string[] subDirectries = Directory.GetDirectories(slnFilePath);

                        foreach (string s in subDirectries)
                        {
                            var ew = s.Remove(0, slnFilePath.Length);
                            actualFileName = ew.Replace(@"\", "");
                        }

                        //save the testcase
                        var filePath = Server.MapPath(@"~/TestCase/" + slnName + ".xml");
                        var fileInfo = new FileInfo(filePath);
                        fileInfo.Directory.Create();
                        testCaseUpload.SaveAs(filePath);

                        ////get the language and pass into grader
                        ProgLanguage lang = db.ProgLanguages.ToList().Find(l => l.LanguageId == db.Modules.ToList().Find(m => m.ModuleCode == addAssgn.ModuleId).LanguageId);

                        Grader g = new Grader(slnFilePath, actualFileName, slnName, lang.LangageType, true);

                        if (g.RunLecturerSolution() == true)
                        {
                            //save to DB 
                            AddAssignmentToDB(addAssgn, true);

                            //get the ID to rename the files
                            var assgnID = db.Assignments.Where(a => a.AssgnTitle == addAssgn.AssgnTitle).Select(s => s.AssignmentID).ToList().First();

                            // rename the file
                            string testCasePath = Server.MapPath(@"~/TestCase/");
                            string solutionPath = Server.MapPath(@"~/Solutions/");

                            foreach (FileInfo f in new DirectoryInfo(solutionPath).GetFiles())
                            {
                                if (f.Name == slnName + ".xml") //to be renamed
                                {
                                    var sourcePath = solutionPath + f.Name;
                                    var destPath = solutionPath + assgnID + "solution.xml";
                                    FileInfo info = new FileInfo(sourcePath);
                                    info.MoveTo(destPath);

                                    var query = from Assignment in db.Assignments where Assignment.AssignmentID == assgnID select Assignment;

                                    foreach (Assignment a in query)
                                    {
                                        a.Solution = assgnID + "solution.xml";
                                    }
                                    db.SaveChanges();
                                }
                            }

                            //rename the testcase
                            foreach (FileInfo f in new DirectoryInfo(testCasePath).GetFiles())
                            {
                                if (f.Name == slnName + ".xml")
                                {
                                    var sourcePath = testCasePath + f.Name;
                                    var destPath = testCasePath + assgnID + "testcase.xml";
                                    FileInfo info = new FileInfo(sourcePath);
                                    info.MoveTo(destPath);
                                }
                            }

                            //delete the uploaded sln
                            DeleteFileWhenError(slnName, false);
                        }
                        else
                        {
                            //solution failed to run 
                            DeleteFileWhenError(slnName, true);
                            addAssgn.Modules = db.Modules.ToList();
                            TempData["GeneralError"] = "Failed to run solution. Please reupload and try again.";
                            return View(addAssgn);
                        }
                    }
                    else
                    {
                        //uploaded file is empty 
                        addAssgn.Modules = db.Modules.ToList();
                        string err = "Uploaded file is invalid ! Please try again.";
                        TempData["SlnWarning"] = err;
                        TempData["TcWarning"] = err;
                        return View(addAssgn);
                    }
                }
                else
                {
                    //uploaded file is invalid
                    addAssgn.Modules = db.Modules.ToList();
                    string err = "Uploaded file is invalid ! Please try again.";
                    TempData["SlnWarning"] = err;
                    TempData["TcWarning"] = err;
                    return View(addAssgn);
                }
            }

            //run without testcase 
            else if (addAssgn.IsTestCasePresent == false)
            {
                if (solutionsFileUpload != null && Path.GetExtension(solutionsFileUpload.FileName) == ".zip")
                {
                    if (solutionsFileUpload.ContentLength > 0)
                    {
                        string slnFilePath = "";
                        string slnName = (addAssgn.AssgnTitle).Replace(" ", "");
                        string actualFileName = "";

                        //save the solution
                        var zipLocation = Server.MapPath(@"~/TempSubmissions/" + solutionsFileUpload);
                        solutionsFileUpload.SaveAs(zipLocation);
                        slnFilePath = Server.MapPath(@"~/TempSubmissions/" + slnName);
                        DirectoryInfo fileDirectory = new DirectoryInfo(slnFilePath);
                        if (fileDirectory.Exists)
                        {
                            foreach (FileInfo files in fileDirectory.GetFiles())
                            {
                                files.Delete();
                            }
                            foreach (DirectoryInfo dir in fileDirectory.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                        }
                        fileDirectory.Create();
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipLocation, slnFilePath);

                        //get the actual folder name containing the submission
                        string[] subDirectries = Directory.GetDirectories(slnFilePath);

                        foreach (string s in subDirectries)
                        {
                            var ew = s.Remove(0, slnFilePath.Length);
                            actualFileName = ew.Replace(@"\", "");
                        }

                        //get the language and pass into grader
                        ProgLanguage lang = db.ProgLanguages.ToList().Find(l => l.LanguageId == db.Modules.ToList().Find(m => m.ModuleCode == addAssgn.ModuleId).LanguageId);

                        Grader g = new Grader(slnFilePath, actualFileName, slnName, lang.LangageType, false);

                        if (g.RunLecturerSolution() == true)
                        {
                            //save to DB 
                            AddAssignmentToDB(addAssgn, false);

                            //get the ID to rename the files
                            var assgnID = db.Assignments.Where(a => a.AssgnTitle == addAssgn.AssgnTitle).Select(s => s.AssignmentID).ToList().First();

                            // rename the file
                            string solutionPath = Server.MapPath(@"~/Solutions/");

                            foreach (FileInfo f in new DirectoryInfo(solutionPath).GetFiles())
                            {
                                if (f.Name == slnName + ".xml") //to be renamed
                                {
                                    var sourcePath = solutionPath + f.Name;
                                    var destPath = solutionPath + assgnID + "solution.xml";
                                    FileInfo info = new FileInfo(sourcePath);
                                    info.MoveTo(destPath);

                                    var query = from Assignment in db.Assignments where Assignment.AssignmentID == assgnID select Assignment;

                                    foreach (Assignment a in query)
                                    {
                                        a.Solution = assgnID + "solution.xml";
                                    }
                                    db.SaveChanges();
                                }
                            }

                            //delete the uploaded sln
                            DeleteFileWhenError(slnName, false);

                        }
                        else
                        {
                            //solution failed to run 
                            DeleteFileWhenError(slnName, false);
                            addAssgn.Modules = db.Modules.ToList();
                            TempData["GeneralError"] = "Failed to run solution. Please reupload and try again.";
                            return View(addAssgn);
                        }
                    }
                    else
                    {
                        //uploaded file is empty 
                        addAssgn.Modules = db.Modules.ToList();
                        TempData["SlnWarning"] = "Uploaded file is empty ! Please try again.";
                        return View(addAssgn);
                    }
                }
                else
                {
                    //uploaded file is invalid 
                    addAssgn.Modules = db.Modules.ToList();
                    TempData["SlnWarning"] = "Uploaded file is invalid ! Please try again.";
                    return View(addAssgn);
                }
            }

            //everything all okay 
            return RedirectToAction("Dashboard", "Lecturer");
        }

        //used to insert the data into DB. 
        public ActionResult AddAssignmentToDB(AddAssignmentViewModel addAssgn, bool isTestCase)
        {
            //now to add into the DB
            Assignment newAssignment = new Assignment();
            Class_Assgn classAssgn = new Class_Assgn();
            List<HttpPostedFileBase> assgnFiles = new List<HttpPostedFileBase>();
            string slnName = (addAssgn.AssgnTitle).Replace(" ", "");

            addAssgn.Solution = "~/Solutions/" + slnName + ".xml";

            newAssignment.AssgnTitle = addAssgn.AssgnTitle;
            newAssignment.Describe = addAssgn.Describe;
            newAssignment.MaxAttempt = addAssgn.MaxAttempt;
            newAssignment.StartDate = addAssgn.StartDate;
            newAssignment.DueDate = addAssgn.DueDate;
            newAssignment.Solution = addAssgn.Solution;
            newAssignment.ModuleCode = addAssgn.ModuleId;
            // newAssignment.CreateBy = "s1431489";
            newAssignment.CreateBy = User.Identity.GetUserName();
            newAssignment.CreateAt = DateTime.Now;
            //newAssignment.UpdatedBy = "s1431489";
            newAssignment.UpdatedBy = User.Identity.GetUserName();
            newAssignment.UpdatedAt = DateTime.Now;
            db.Assignments.Add(newAssignment);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                DeleteFileWhenError(slnName, isTestCase);
                addAssgn.Modules = db.Modules.ToList();
                TempData["GeneralError"] = "Failed to add assignment. Please try again.";
                return View(addAssgn);
            }

            //get the assignment ID 
            var assgnId = db.Assignments.Where(a => a.AssgnTitle == addAssgn.AssgnTitle).Select(s => s.AssignmentID).ToList().First();

            //insert into Class_Assgn
            foreach (AssignmentClass a in addAssgn.ClassList)
            {
                if (a.isSelected == true)
                {
                    classAssgn.ClassID = a.ClassId;
                    classAssgn.AssignmentID = assgnId;
                    db.Class_Assgn.Add(classAssgn);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        DeleteFileWhenError(slnName, isTestCase);
                        addAssgn.Modules = db.Modules.ToList();
                        TempData["GeneralError"] = "Failed to add assignment. Please try again.";
                        return View(addAssgn);
                    }
                }
            }
            return null;
        }

        //used to delete files when something goes wrong somewhere
        private void DeleteFileWhenError(string assgnTitle, bool isTestCase)
        {
            //delete their solution + testcase 
            var tempPath = Server.MapPath(@"~/TempSubmissions/" + assgnTitle);
            var tempPath2 = Server.MapPath(@"~/TestCase/");

            DirectoryInfo di;

            if ((di = new DirectoryInfo(tempPath)).Exists)
            {
                foreach (FileInfo f in di.GetFiles())
                {
                    f.IsReadOnly = false;
                    f.Delete();
                }

                foreach (DirectoryInfo dr in di.GetDirectories())
                {
                    dr.Delete(true);
                }

                di.Delete();
            }

            //delete testcase IF present
            if (isTestCase == true)
            {
                if ((di = new DirectoryInfo(tempPath2)).Exists)
                {
                    foreach (FileInfo f in di.GetFiles())
                    {
                        if (f.Name == assgnTitle + ".xml")
                        {
                            f.IsReadOnly = false;
                            f.Delete();
                        }
                    }
                }
            }
        }

        public ActionResult UpdateAssignment(int AssignmentId)
        {
            return View();
        }

        public ActionResult ViewResults()
        {
            ViewResultsViewModel vrvm = new ViewResultsViewModel();

            string loggedInLecturer = User.Identity.GetUserName(); //temp 


            List<Class> managedClasses = db.Classes.Where(c2 => c2.DeletedAt == null).Where(c => c.Lec_Class.Where(lc => lc.ClassID == c.ClassID).FirstOrDefault().StaffID == loggedInLecturer).ToList();

            List<String> classIds = new List<String>();
            List<String> classNames = new List<String>();

            foreach (Class c in managedClasses)
            {
                Course course = db.Courses.Where(courses => courses.CourseID == c.CourseID).FirstOrDefault();

                String cId = c.ClassID.ToString();
                String cName = course.CourseAbbr + "/" + c.ClassName.ToString();

                classIds.Add(cId);
                classNames.Add(cName);
            }

            vrvm.classIds = classIds;
            vrvm.classNames = classNames;

            return View(vrvm);

        }

        [HttpPost]
        public ActionResult GetAssignment(string Class)
        {
            string loggedInLecturer = User.Identity.GetUserName(); //temp 

            var assignments = db.Database.SqlQuery<DBass>("select ca.*, a.AssgnTitle from Class_Assgn ca inner join(select * from Assignment) a on ca.AssignmentID = a.AssignmentID where classid = @inClass and createby = @inCreator and deletedat is null",
    new SqlParameter("@inClass", Class),
    new SqlParameter("@inCreator", loggedInLecturer)).ToList();

            return Json(assignments);
        }

        [HttpPost]
        public ActionResult ViewResults(string Class, string Assignment)
        {


            var results = db.Database.SqlQuery<DBres>("select s1.submissionid, s1.adminno, stud.name, s1.assignmentid, s1.grade, s1.filepath from submission s1 inner join ( select adminno, max(submissionid) submissionid, assignmentid from submission group by adminno, assignmentid) s2 on s1.submissionid = s2.submissionid inner join ( select * from student where classid = @inClass) stud on s1.adminno = stud.adminno where s1.assignmentid = @inAssignment",
    new SqlParameter("@inClass", Class),
    new SqlParameter("@inAssignment", Assignment)).ToList();

            return Json(results);
        }

        [HttpGet]
        public ActionResult Download(string file)
        {

            string path = "~/Submissions/" + file;
            string zipname = file + ".zip";

            var memoryStream = new MemoryStream();
            using (var zip = new ZipFile())
            {

                zip.AddDirectory(Server.MapPath(path));
                zip.Save(memoryStream);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/zip", zipname);


        }

        class DBass
        {
            public int ClassID { get; set; }
            public int AssignmentID { get; set; }
            public int _id { get; set; }
            public string AssgnTitle { get; set; }
        }

        class DBres
        {
            public int submissionid { get; set; }
            public string adminno { get; set; }
            public string name { get; set; }
            public int assignmentid { get; set; }
            public decimal grade { get; set; }
            public string filepath { get; set; }
        }

    }//end of controller

}