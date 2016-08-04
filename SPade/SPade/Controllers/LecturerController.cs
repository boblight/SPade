using Ionic.Zip;
using Microsoft.AspNet.Identity;
using SPade.Grading;
using SPade.Models.DAL;
using SPade.ViewModels.Lecturer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        [HttpGet]
        public ActionResult BulkAddStudent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BulkAddStudent(HttpPostedFileBase file)
        {
            if ((file != null && Path.GetExtension(file.FileName) == ".csv") && (file.ContentLength > 0))
            {
                //Upload and save the file
                // extract only the filename
                var fileName = Path.GetFileName(file.FileName);
            // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
                file.SaveAs(path);

                string[] lines = System.IO.File.ReadAllLines(path);
                List<Student> slist = new List<Student>();
                for (int i = 1; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                    {
                        Student s = new Student();
                        s.ClassID = Int32.Parse(lines[i].Split(',')[0]);
                        s.AdminNo = lines[i].Split(',')[1];
                        s.Name = lines[i].Split(',')[2];
                        s.Email = lines[i].Split(',')[3];
                        s.ContactNo = Int32.Parse(lines[i].Split(',')[4]);
                        s.CreatedAt = DateTime.Now;
                        s.CreatedBy = User.Identity.GetUserName();
                        s.UpdatedAt = DateTime.Now;
                        s.UpdatedBy = User.Identity.GetUserName();

                        slist.Add(s);
                    }
                }
                db.Students.AddRange(slist);
                db.SaveChanges();
            }else
            {
                // Upload file is invalid
                string err = "Uploaded file is invalid ! Please try again!";
                TempData["InputWarning"] = err;
                return View();
            }
    
            return View("ManageClassesAndStudents");
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
            var lecturerID = User.Identity.GetUserName();

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

            aaVM.IsTestCasePresent = true;
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
                        if (solutionsFileUpload.ContentLength > 104857600)
                        {
                            //SubmitWithTestCase(addAssgn, solutionsFileUpload, testCaseUpload);
                            string slnFilePath = "";
                            string assignmentTitle = (addAssgn.AssgnTitle).Replace(" ", ""); //used to name the testcase/solution temporarily until get assignmentID 
                            string fileName = ""; //name of the solution uploaded

                            //save the solution
                            fileName = Path.GetFileNameWithoutExtension(solutionsFileUpload.FileName);
                            var zipLocation = Server.MapPath(@"~/TempSubmissions/" + solutionsFileUpload);
                            solutionsFileUpload.SaveAs(zipLocation);
                            slnFilePath = Server.MapPath(@"~/TempSubmissions/" + fileName);
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

                            //access the solution + move the classname.java/.cs into a folder 
                            var toLowerPath = fileName.ToLower();
                            var path = System.IO.Path.Combine(slnFilePath, toLowerPath);
                            Directory.CreateDirectory(path);

                            ////get the language and pass into grader
                            ProgLanguage lang = db.ProgLanguages.ToList().Find(l => l.LanguageId == db.Modules.ToList().Find(m => m.ModuleCode == addAssgn.ModuleId).LanguageId);

                            var ogPath = "";
                            var newPath = "";

                            if (lang.LangageType.Equals("Java"))
                            {
                                ogPath = slnFilePath + "/" + fileName + ".java";
                                newPath = path + "/" + fileName + ".java";
                            }
                            else if (lang.LangageType.Equals("C#"))
                            {
                                ogPath = slnFilePath + "/" + fileName + ".cs";
                                newPath = path + "/" + fileName + ".cs";
                            }

                            System.IO.File.Move(ogPath, newPath);

                            //save the testcase
                            var filePath = Server.MapPath(@"~/TestCase/" + assignmentTitle + ".xml");
                            var fileInfo = new FileInfo(filePath);
                            fileInfo.Directory.Create();
                            testCaseUpload.SaveAs(filePath);

                            //get the language and pass into grader

                            Grader g = new Grader(slnFilePath, fileName, assignmentTitle, lang.LangageType, true);

                            //change running of lecturer from checking boolean to checking exitcode
                            //1 is successfully done everything
                            //2 is test case submitted could not be read
                            //3 is program has failed to run
                            //4 is program was caught in an infinite loop
                            int exitCode = g.RunLecturerSolution();

                            if (exitCode == 1)
                            {
                                //save to DB + rename solution/testcase
                                AddAssignmentToDB(addAssgn, fileName, true);

                                //delete the uploaded sln
                                DeleteFile(fileName, assignmentTitle, false);

                            }//end of run succesfully method 

                            else if (exitCode == 2)
                            {
                                DeleteFile(fileName, assignmentTitle, true);
                                addAssgn.Modules = db.Modules.ToList();
                                TempData["GeneralError"] = "The test case submitted could not be read properly. Please check your test case file";
                                return View(addAssgn);
                            }

                            else if (exitCode == 3)
                            {
                                //solution failed to run 
                                DeleteFile(fileName, assignmentTitle, true);
                                addAssgn.Modules = db.Modules.ToList();
                                TempData["GeneralError"] = "The program has failed to run entirely. Please check your program";
                                return View(addAssgn);
                            }
                            else if (exitCode == 4)
                            {
                                //solution stuck in infinite loop
                                DeleteFile(fileName, assignmentTitle, true);
                                addAssgn.Modules = db.Modules.ToList();
                                TempData["GeneralError"] = "The program uploaded was caught in an infinite loop. Please check your program";
                                return View(addAssgn);
                            }
                        }
                        else
                        {
                            //uploaded file is more than 150MB
                            addAssgn.Modules = db.Modules.ToList();
                            TempData["SlnWarning"] = "Please make sure that your files is less than 150MB !";
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
            }//end of run with testcase

            //run without testcase 
            else if (addAssgn.IsTestCasePresent == false)
            {
                if (solutionsFileUpload != null && Path.GetExtension(solutionsFileUpload.FileName) == ".zip")
                {
                    if (solutionsFileUpload.ContentLength > 0)
                    {
                        string slnFilePath = "";
                        string assignmentTitle = (addAssgn.AssgnTitle).Replace(" ", "");
                        string fileName = "";

                        //save the solution
                        fileName = Path.GetFileNameWithoutExtension(solutionsFileUpload.FileName);
                        var zipLocation = Server.MapPath(@"~/TempSubmissions/" + solutionsFileUpload);
                        solutionsFileUpload.SaveAs(zipLocation);
                        slnFilePath = Server.MapPath(@"~/TempSubmissions/" + fileName);
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

                        //access the solution + move the classname.java/.cs into a folder 
                        var toLowerPath = fileName.ToLower();
                        var path = System.IO.Path.Combine(slnFilePath, toLowerPath);
                        Directory.CreateDirectory(path);

                        //get the language and pass into grader
                        ProgLanguage lang = db.ProgLanguages.ToList().Find(l => l.LanguageId == db.Modules.ToList().Find(m => m.ModuleCode == addAssgn.ModuleId).LanguageId);

                        //get the appropirate file and move the file accordingly
                        var ogPath = "";
                        var newPath = "";

                        if (lang.LangageType.Equals("Java"))
                        {
                            ogPath = slnFilePath + "/" + fileName + ".java";
                            newPath = path + "/" + fileName + ".java";
                        }
                        else if (lang.LangageType.Equals("C#"))
                        {
                            ogPath = slnFilePath + "/" + fileName + ".cs";
                            newPath = path + "/" + fileName + ".cs";
                        }

                        System.IO.File.Move(ogPath, newPath);

                        Grader g = new Grader(slnFilePath, fileName, assignmentTitle, lang.LangageType, false);

                        int exitCode = g.RunLecturerSolution();

                        if (exitCode == 1)
                        {
                            //save to DB + rename solution file
                            AddAssignmentToDB(addAssgn, fileName, false);

                            //delete the uploaded sln
                            DeleteFile(fileName, assignmentTitle, false);

                        }
                        else if (exitCode == 3)
                        {
                            //solution failed to run 
                            DeleteFile(fileName, assignmentTitle, false);
                            addAssgn.Modules = db.Modules.ToList();
                            TempData["GeneralError"] = "The program uploaded was caught in an infinite loop. Please check your program";
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
            }//end of run without testcase

            //everything all okay 
            return RedirectToAction("ManageAssignments", "Lecturer");
        }

        //used to move the class file into the subfolder in order for it to be compiled
        //THIS IS BROKEN. PAY NO MIND TO IT

        //public bool MoveFileToSubFolder(string fileName, string slnFilePath, string assignmentTitle, ProgLanguage lang, bool isTestCasePresent)
        //{
        //    bool saveStatus = false;

        //    try
        //    {
        //        //access the solution + move the classname.java/.cs into a folder 
        //        var toLowerPath = fileName.ToLower();
        //        var path = System.IO.Path.Combine(slnFilePath, toLowerPath);
        //        Directory.CreateDirectory(path);

        //        //get the appropirate file and move the file accordingly
        //        var ogPath = "";
        //        var newPath = "";

        //        if (lang.LangageType.Equals("Java"))
        //        {
        //            ogPath = slnFilePath + "/" + fileName + ".java";
        //            newPath = path + "/" + fileName + ".java";
        //        }
        //        else if (lang.LangageType.Equals("C#"))
        //        {
        //            ogPath = slnFilePath + "/" + fileName + ".cs";
        //            newPath = path + "/" + fileName + ".cs";
        //        }

        //        System.IO.File.Move(ogPath, newPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        saveStatus = true;
        //    }
        //    return saveStatus;
        //}

        //used to insert the data into DB. 
        public ActionResult AddAssignmentToDB(AddAssignmentViewModel addAssgn, string fileName, bool isTestCase)
        {
            //now to add into the DB
            Assignment newAssignment = new Assignment();
            Class_Assgn classAssgn = new Class_Assgn();
            List<HttpPostedFileBase> assgnFiles = new List<HttpPostedFileBase>();
            string assignmentTitle = (addAssgn.AssgnTitle).Replace(" ", "");

            addAssgn.Solution = "~/Solutions/" + assignmentTitle + ".xml";

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
                DeleteFile(fileName, assignmentTitle, isTestCase);
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
                        DeleteFile(fileName, assignmentTitle, isTestCase);
                        addAssgn.Modules = db.Modules.ToList();
                        TempData["GeneralError"] = "Failed to add assignment. Please try again.";
                        return View("AddAssignment", addAssgn);
                    }
                }
            }

            //rename the solution here
            var solutionPath = Server.MapPath(@"~/Solutions/");

            foreach (FileInfo f in new DirectoryInfo(solutionPath).GetFiles())
            {
                if (f.Name == assignmentTitle + ".xml")
                {
                    var sourcePath = solutionPath + f.Name;
                    var destPath = solutionPath + assgnId + "solution.xml";
                    FileInfo info = new FileInfo(sourcePath);
                    info.MoveTo(destPath);

                    var query = from Assignment in db.Assignments where Assignment.AssignmentID == assgnId select Assignment;

                    //update the DB to reflect the change of name for the solution
                    foreach (Assignment a in query)
                    {
                        a.Solution = "~/Solutions/" + assgnId + "solution.xml";
                    }
                    db.SaveChanges();
                }
            }

            //rename the testcase IF there is one 
            if (isTestCase == true)
            {
                var testCasePath = Server.MapPath(@"~/TestCase/");

                foreach (FileInfo f in new DirectoryInfo(testCasePath).GetFiles())
                {
                    if (f.Name == assignmentTitle + ".xml")
                    {
                        var sourcePath = testCasePath + f.Name;
                        var destPath = testCasePath + assgnId + "testcase.xml";
                        FileInfo info = new FileInfo(sourcePath);
                        info.MoveTo(destPath);
                    }
                }
            }

            return null;
        }

        //used to delete files when something goes wrong somewhere
        private void DeleteFile(string fileName, string assgnTitle, bool isTestCase)
        {
            //delete their solution + testcase 
            var tempPath = Server.MapPath(@"~/TempSubmissions/" + fileName);
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

        public ActionResult UpdateAssignment()
        {

            //create model to store info to display
            UpdateAssignmentViewModel model = new UpdateAssignmentViewModel();
            //load lecAssgn list with assignments.
            List<Assignment> lecAssgn = db.Assignments.ToList();
            List<Class_Assgn> classList = db.Class_Assgn.ToList();

            int x = 2;

            List<string> classAssgn = new List<string>();

            //get the name of the classes assigned to an assignment 
            foreach (Assignment a in lecAssgn)
            {
                if (a.AssignmentID.Equals(x))
                {
                    model.Assignment = a;
                    model.AssgnTitle = a.AssgnTitle;
                    model.Describe = a.Describe;
                    model.Solution = a.Solution;
                    model.ModuleId = a.ModuleCode;
                    model.StartDate = a.StartDate;
                    model.DueDate = a.DueDate;
                    model.MaxAttempt = a.MaxAttempt;

                }

            }




            return View(model);
        }

        [HttpPost]
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