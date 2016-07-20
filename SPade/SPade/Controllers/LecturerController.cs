using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Grading;
using SPade.Models.DAL;
using SPade.ViewModels.Admin;
using SPade.ViewModels.Lecturer;
using SPade.ViewModels.Student;
using System.IO;
using System.Data.SqlClient;
using Ionic.Zip;
using Microsoft.AspNet.Identity;

namespace SPade.Controllers
{
    public class LecturerController : Controller
    {
        //init the db
        private SPadeDBEntities db = new SPadeDBEntities();

        // [Authorize(Roles = "")]
        // GET: Lecturer
        public ActionResult Dashboard()
        {
            return View();
        }

        //[Authorize(Roles = "")]
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

        // [Authorize(Roles = "")]
        public ActionResult BulkAddStudent()
        {
            return View();
        }

        // [Authorize(Roles = "")]
        public ActionResult ViewStudentsByClass(string classID)
        {
            return View();
        }

        //  [Authorize(Roles = "")]
        public ActionResult UpdateStudent()
        {
            return View();
        }

        //   [Authorize(Roles = "")]
        public ActionResult ManageAssignments()
        {
            return View();
        }

        public FileResult DownloadTestCase()
        {
            string f = Server.MapPath(@"~/TestCase/testcase.xml");
            byte[] fileBytes = System.IO.File.ReadAllBytes(f);
            string fileName = "testcase.xml";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        //   [Authorize(Roles = "")]
        public ActionResult AddAssignment()
        {

            List<AssignmentClass> ac = new List<AssignmentClass>();
            AddAssignmentViewModel aaVM = new AddAssignmentViewModel();

            string x = "s1431489"; //temp 

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
        //  [Authorize(Roles = "")]
        public ActionResult AddAssignment(AddAssignmentViewModel addAssgn, HttpPostedFileBase solutionsFileUpload, HttpPostedFileBase testCaseUpload)
        {
            string slnFilePath = "", slnFileName = "";

            //check the solutions file and the testcase file 
            if ((solutionsFileUpload != null && Path.GetExtension(solutionsFileUpload.FileName) == ".zip") && (testCaseUpload != null && Path.GetExtension(testCaseUpload.FileName) == ".xml"))
            {
                if (solutionsFileUpload.ContentLength > 0 && testCaseUpload.ContentLength > 0)
                {
                    //unzip and save the solution 
                    slnFileName = Path.GetFileNameWithoutExtension(solutionsFileUpload.FileName);
                    var zipLocation = Server.MapPath(@"~/TempSubmissions/" + solutionsFileUpload);
                    solutionsFileUpload.SaveAs(zipLocation);
                    slnFilePath = Server.MapPath(@"~/TempSubmissions/" + addAssgn.AssgnTitle);
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

                    //save the testcase
                    var fileName = Path.GetFileName(testCaseUpload.FileName);
                    var filePath = Server.MapPath(@"~/TestCase/" + addAssgn.AssgnTitle + ".xml");
                    var fileInfo = new FileInfo(filePath);
                    fileInfo.Directory.Create();
                    testCaseUpload.SaveAs(filePath);

                    //run the lecturer solution + generate solution file 
                    Grader g = new Grader(slnFilePath, slnFileName, addAssgn.AssgnTitle);
                    if (g.RunLecturerSolution() == true)
                    {
                        //now to add into the DB 
                        Assignment newAssignment = new Assignment();
                        Class_Assgn classAssgn = new Class_Assgn();
                        List<HttpPostedFileBase> assgnFiles = new List<HttpPostedFileBase>();

                        addAssgn.Solution = addAssgn.AssgnTitle + ".xml";

                        newAssignment.AssgnTitle = addAssgn.AssgnTitle;
                        newAssignment.Describe = addAssgn.Describe;
                        newAssignment.MaxAttempt = addAssgn.MaxAttempt;
                        newAssignment.StartDate = addAssgn.StartDate;
                        newAssignment.DueDate = addAssgn.DueDate;
                        newAssignment.Solution = addAssgn.Solution;
                        newAssignment.ModuleCode = addAssgn.ModuleId;
                        newAssignment.CreateBy = "1431485"; //temp
                        newAssignment.CreateAt = DateTime.Now;
                        newAssignment.UpdatedBy = "1431485"; //temp
                        newAssignment.UpdatedAt = DateTime.Now;
                        db.Assignments.Add(newAssignment);

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            addAssgn.Modules = db.Modules.ToList();
                            TempData["GeneralError"] = "Failed to add assignment. Please try again.";
                            return View(addAssgn);
                        }

                        var assgnID = db.Assignments.Where(a => a.AssgnTitle == addAssgn.AssgnTitle).Select(s => s.AssignmentID).ToList().First(); //get the ID

                        //insert into Class_Assgn
                        foreach (AssignmentClass a in addAssgn.ClassList)
                        {
                            if (a.isSelected == true)
                            {
                                classAssgn.ClassID = a.ClassId;
                                classAssgn.AssignmentID = assgnID;
                                db.Class_Assgn.Add(classAssgn);

                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    addAssgn.Modules = db.Modules.ToList();
                                    TempData["GeneralError"] = "Failed to add assignment. Please try again.";
                                    return View(addAssgn);
                                }
                            }
                        }

                        //rename the file
                        string testCasePath = Server.MapPath(@"~/TestCase/");
                        string solutionPath = Server.MapPath(@"~/Solutions/");

                        foreach (FileInfo f in new DirectoryInfo(solutionPath).GetFiles())
                        {
                            if (f.Name == addAssgn.AssgnTitle + ".xml") //to be renamed
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
                            if (f.Name == addAssgn.AssgnTitle + ".xml")
                            {
                                var sourcePath = testCasePath + f.Name;
                                var destPath = testCasePath + assgnID + "testcase.xml";
                                FileInfo info = new FileInfo(sourcePath);
                                info.MoveTo(destPath);
                            }
                        }

                        //delete the uploaded sln
                        string slnPath = Server.MapPath(@"~/TempSubmissions/" + addAssgn.AssgnTitle);

                        DirectoryInfo slnDirectory = new DirectoryInfo(slnPath);
                        if (slnDirectory.Exists)
                        {
                            foreach(FileInfo f in slnDirectory.GetFiles())
                            {
                                f.Delete(); 
                            }
                            foreach(DirectoryInfo dr in slnDirectory.GetDirectories())
                            {
                                dr.Delete(); 
                            }
                        } 

                        //foreach (FileInfo f in new DirectoryInfo(slnPath).GetFiles())
                        //{
                        //    if (f.Name == addAssgn.AssgnTitle)
                        //    {
                        //        f.Delete();
                        //    }
                        //}

                    }
                    else
                    {
                        //failed to run their solution
                        addAssgn.Modules = db.Modules.ToList();
                        TempData["GeneralError"] = "Failed to run solution. Please reupload and try again.";
                        return View(addAssgn);
                    }
                }
                else
                {
                    //uploaded file got sumthing wong
                    addAssgn.Modules = db.Modules.ToList();
                    TempData["Warning"] = "Uploaded file is invalid ! Please try again.";
                    return View(addAssgn);
                }
            }
            else
            {
                //invalid file extension or null
                addAssgn.Modules = db.Modules.ToList();
                TempData["Warning"] = "Uploaded file is invalid ! Please try again.";
                return View(addAssgn);
            }

            return View("ManageAssignments");
        }

        //  [Authorize(Roles = "")]
        public ActionResult UpdateAssignment()
        {
            return View();
        }

        //    [Authorize(Roles = "")]
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