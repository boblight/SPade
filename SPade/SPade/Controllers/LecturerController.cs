using Ionic.Zip;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SPade.Grading;
using SPade.Models;
using SPade.Models.DAL;
using SPade.ViewModels.Lecturer;
using SPade.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using Newtonsoft.Json;
using System.Xml;
using System.Text.RegularExpressions;

namespace SPade.Controllers
{

    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        //init the db
        private SPadeDBEntities db = new SPadeDBEntities();
        private ApplicationUserManager _userManager;

        public LecturerController()
        {

        }

        public LecturerController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        //Manage Classes & Manage + Update Student
        public ActionResult ManageClassesAndStudents()
        {
            List<ManageClassesViewModel> manageClassView = new List<ManageClassesViewModel>();


            string loggedInLecturer = User.Identity.GetUserName();

            //get the classes managed by the lecturer 


            List<Class> managedClasses = new List<Class>();

            List<Lec_Class> lec_classes = db.Lec_Class.Where(lc => lc.StaffID == loggedInLecturer).ToList();

            foreach (Lec_Class lc in lec_classes)
            {
                List<Class> temp = db.Classes.Where(c => c.DeletedAt == null).Where(c => c.ClassID == lc.ClassID).ToList();
                managedClasses.AddRange(temp);
            }




            //get the students in that classs
            foreach (Class c in managedClasses)
            {

                ManageClassesViewModel e = new ManageClassesViewModel();
                //match the class ID of student wit hthe class ID of the managed Classes
                var count = db.Students.Where(s => s.ClassID == c.ClassID).Where(s => s.DeletedAt == null).Count();

                Course cs = db.Courses.Where(cx => cx.CourseID == c.CourseID).First();

                e.ClassName = cs.CourseAbbr + "/" + c.ClassName;
                e.Id = c.ClassID;
                e.NumberOfStudents = count;

                manageClassView.Add(e);

            }

            return View(manageClassView);

        }

        public ActionResult ViewStudentsByClass(string Id)
        {
            int cID = Int32.Parse(Id);

            List<ViewStudentsByClassViewModel> studList = new List<ViewStudentsByClassViewModel>();
            List<Student> sList = new List<Student>();

            sList = db.Students.Where(s => s.ClassID == cID && s.DeletedAt == null).ToList();

            Class c = db.Classes.Where(cx => cx.ClassID == cID).FirstOrDefault();

            int courseId = c.CourseID;
            string courseAbbr = db.Courses.ToList().Find(cx => cx.CourseID == courseId).CourseAbbr;

            foreach (Student s in sList)
            {
                ViewStudentsByClassViewModel vm = new ViewStudentsByClassViewModel();

                vm.AdminNo = s.AdminNo.ToUpper();
                vm.Name = s.Name;
                vm.Email = s.Email;
                vm.ContactNo = s.ContactNo;

                studList.Add(vm);
            }

            ViewBag.ClassName = courseAbbr + "/" + c.ClassName;
            return View(studList);
        }

        public ActionResult UpdateStudent(string id)
        {
            ViewModels.Lecturer.UpdateStudentViewModel model = new ViewModels.Lecturer.UpdateStudentViewModel();

            //Get all classes
            List<Class> allClasses = db.Classes.Where(cl => cl.DeletedAt == null).ToList();

            foreach (Class c in allClasses)
            {
                String courseAbbr = db.Courses.Where(courses => courses.CourseID == c.CourseID).FirstOrDefault().CourseAbbr;
                String className = courseAbbr + "/" + c.ClassName;

                c.ClassName = className;
            }

            model.Classes = allClasses;

            //Get Student           
            Student student = db.Students.ToList().Find(st => st.AdminNo == id);

            model.AdminNo = student.AdminNo;
            model.Name = student.Name;
            model.ClassID = student.ClassID;
            model.ContactNo = student.ContactNo;
            model.Email = student.Email;
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateStudent(ViewModels.Lecturer.UpdateStudentViewModel model, string command, string AdminNo)
        {
            //Get all classes
            List<Class> allClasses = db.Classes.ToList();
            model.Classes = allClasses;
            Student student = db.Students.Where(s => s.AdminNo == AdminNo).FirstOrDefault();

            //Udate Student information
            if (command.Equals("Update"))
            {
                student.Name = model.Name;
                student.ContactNo = model.ContactNo;
                student.ClassID = model.ClassID;
                student.UpdatedAt = DateTime.Now;
                student.UpdatedBy = User.Identity.Name;
                db.SaveChanges();
                return RedirectToAction("ManageClassesAndStudents");
            }
            //Delete Student
            else
            {
                if (db.Submissions.Where(sub => sub.AdminNo == AdminNo).Count() == 0)
                {
                    student.DeletedAt = DateTime.Now;
                    student.DeletedBy = User.Identity.Name;

                    db.SaveChanges();
                    return RedirectToAction("ManageClassesAndStudents");
                }
                else
                {
                    ModelState.AddModelError("DeleteError", "There are still submissions that are tied to this student's account. " +
                        "Contact an Admin to purge all submissions made by this student before deleting.");
                    return View(model);
                }
            }
        }

        //Add Students (Bulk + Single)
        public FileResult DownloadBulkAddStudentFile()
        {
            string f = Server.MapPath(@"~/BulkUploadFiles/BulkAddStudent.csv");
            byte[] fileBytes = System.IO.File.ReadAllBytes(f);
            string fileName = "BulkAddStudent.csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult BulkAddStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> BulkAddStudent(HttpPostedFileBase file)
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
                        string courseAbbr = lines[i].Split(',')[0];
                        string className = lines[i].Split(',')[1];

                        try
                        {
                            s.ClassID = db.Classes.Where(cl => cl.CourseID == db.Courses.Where(co => co.CourseAbbr.Equals(courseAbbr)).FirstOrDefault().CourseID).ToList().Find(cl => cl.ClassName.Equals(className)).ClassID;
                        }
                        catch (Exception excp)
                        {
                            ModelState.AddModelError("", "There is an invalid course abbreviation or class name");
                            return View();
                        }

                        s.AdminNo = lines[i].Split(',')[2];
                        s.Name = lines[i].Split(',')[3];
                        s.Email = lines[i].Split(',')[4];
                        s.ContactNo = Int32.Parse(lines[i].Split(',')[5]);
                        s.CreatedAt = DateTime.Now;
                        s.CreatedBy = User.Identity.GetUserName();
                        s.UpdatedAt = DateTime.Now;
                        s.UpdatedBy = User.Identity.GetUserName();

                        //check through and validate all details
                        //check staff id
                        var match = Regex.Match(s.AdminNo, "^[p0-9]{8,8}$");
                        if (!match.Success)
                        {
                            ModelState.AddModelError("", "One of the administrative number is invalid");
                            return View();
                        }

                        //check contact no.
                        match = Regex.Match(s.ContactNo.ToString(), "^[0-9]{8,8}$");
                        if (!match.Success)
                        {
                            ModelState.AddModelError("", "One of the contact number is invalid");
                            return View();
                        }

                        slist.Add(s);

                        var user = new ApplicationUser { UserName = s.AdminNo, Email = s.Email };
                        user.EmailConfirmed = true;
                        var result = await UserManager.CreateAsync(user, "P@ssw0rd"); //default password
                        if (result.Succeeded)
                        {
                            UserManager.AddToRole(user.Id, "Student");
                        }
                        else
                        {
                            string errors = "";

                            foreach (string err in result.Errors)
                            {
                                errors += err + "\n";
                            }

                            ModelState.AddModelError("", errors);
                            return View();
                        }
                    }
                }
                db.Students.AddRange(slist);
                db.SaveChanges();
            }
            else
            {
                // Upload file is invalid
                string err = "Uploaded file is invalid! Please try again!";
                TempData["InputWarning"] = err;
                return View();
            }

            return RedirectToAction("ManageClassesAndStudents");
        }

        public ActionResult AddOneStudent()
        {
            AddStudentViewModel model = new AddStudentViewModel();
            List<Class> allClasses = db.Classes.ToList().FindAll(c => c.DeletedAt == null);
            List<string> classnames = new List<string>();
            List<int> classids = new List<int>();

            foreach (Class c in allClasses)
            {
                String courseAbbr = db.Courses.Where(courses => courses.CourseID == c.CourseID).FirstOrDefault().CourseAbbr;
                String className = courseAbbr + "/" + c.ClassName;

                classids.Add(c.ClassID);
                classnames.Add(className);
                //c.ClassName = className;
            }

            model.className = classnames;
            model.classID = classids;
            model.Classes = allClasses;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddOneStudent(AddStudentViewModel model, FormCollection formCollection)
        {
            var user = new ApplicationUser { UserName = model.AdminNo, Email = model.Email };
            user.EmailConfirmed = true;
            var result = await UserManager.CreateAsync(user, "P@ssw0rd"); //default password
            if (result.Succeeded)
            {
                var student = new Student()
                {
                    AdminNo = model.AdminNo.Trim(),
                    Name = model.Name,
                    Email = model.Email,
                    ContactNo = model.ContactNo,
                    ClassID = Int32.Parse(formCollection["ClassID"].ToString()),
                    CreatedBy = User.Identity.Name,
                    UpdatedBy = User.Identity.Name,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.Students.Add(student);
                db.SaveChanges();
            }
            else
            {
                //error in registering account
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(model);
            }
            return RedirectToAction("Dashboard");
        }

        //Manage + Update Assignment
        public ActionResult ManageAssignments()
        {
            List<ManageAssignmentViewModel> manageAssgn = new List<ManageAssignmentViewModel>();
            List<Assignment> lecAssgn = new List<Assignment>();
            List<Class> classAssgn = new List<Class>();
            List<Course> courseList = new List<Course>();

            var lecturerID = User.Identity.GetUserName();

            //get the assignments that this lecturer created
            lecAssgn = db.Assignments.ToList().FindAll(a => a.CreateBy == lecturerID && a.DeletedAt == null);
            courseList = db.Courses.ToList().FindAll(c => c.DeletedAt == null);

            //get the details of each assignment and pass to view
            foreach (Assignment a in lecAssgn)
            {
                ManageAssignmentViewModel mmvm = new ManageAssignmentViewModel();

                //get all the classes that are assigned the particular assignment under this lecturer + the classes they manage only ! 
                var query = from c in db.Classes
                            join ca in db.Class_Assgn on c.ClassID equals ca.ClassID
                            where ca.AssignmentID.Equals(a.AssignmentID)
                            join cl in db.Lec_Class on c.ClassID equals cl.ClassID
                            where cl.StaffID.Equals(lecturerID)
                            where c.DeletedAt == null
                            select c;

                classAssgn = query.ToList();

                string joinedClasses = "";

                //if theres only 1 class assigned that assignment 
                if (classAssgn.Count() == 1)
                {
                    foreach (Course c in courseList)
                    {
                        if (c.CourseID == classAssgn.FirstOrDefault().CourseID)
                        {
                            joinedClasses = c.CourseAbbr + "/" + classAssgn.FirstOrDefault().ClassName;
                        }
                    }
                }

                //if there are more then 1 class assigned that assignment
                if (classAssgn.Count() > 1)
                {
                    //get the last item 
                    var lastItem = classAssgn.LastOrDefault().ClassName;
                    var listSize = classAssgn.Count();
                    int listIndex = 0;

                    foreach (Class c in classAssgn)
                    {
                        listIndex++;

                        if (listIndex != listSize) //not yet the last time
                        {
                            foreach (Course cr in courseList)
                            {
                                if (cr.CourseID == c.CourseID)
                                {
                                    joinedClasses += cr.CourseAbbr + "/" + c.ClassName + ", ";
                                }
                            }
                        }

                        if (listIndex == listSize) //reach the last time 
                        {
                            foreach (Course cr in courseList)
                            {
                                if (cr.CourseID == c.CourseID)
                                {
                                    joinedClasses += cr.CourseAbbr + "/" + c.ClassName;
                                }
                            }
                        }
                    }
                }//end of class loop 

                mmvm.Assignment = a;
                mmvm.Classes = joinedClasses;
                manageAssgn.Add(mmvm);
            }

            return View(manageAssgn);
        }

        public ActionResult UpdateAssignment(string assignmentId)
        {
            var i = Int32.Parse(assignmentId);
            var x = User.Identity.GetUserName();

            UpdateAssignmentViewModel model = new UpdateAssignmentViewModel();
            Assignment assgn = new Assignment();
            List<Module> modList = new List<Module>();

            List<Class> classList = new List<Class>();
            List<Class_Assgn> classAssgn = new List<Class_Assgn>();
            List<AssignmentClass> assgnClassList = new List<AssignmentClass>();
            List<Course> courseList = new List<Course>();

            //get the assignment details from the DB
            assgn = db.Assignments.Where(a => a.AssignmentID == i).Where(assn => assn.DeletedAt == null).FirstOrDefault();

            //get all courses
            courseList = db.Courses.Where(c => c.DeletedAt == null).ToList();

            //get the classes that this lecturer manages 
            var query = from c in db.Classes join lc in db.Lec_Class on c.ClassID equals lc.ClassID where lc.StaffID.Equals(x) where c.DeletedAt == null select c;
            classList = query.ToList();

            //now we get the classses that are assigned this assignment 
            classAssgn = db.Class_Assgn.Where(ca => ca.AssignmentID == i).ToList();

            //loop through the classes, check if they are assigned, then give to assignClassList to populate the checkboxes
            foreach (Class cl in classList)
            {
                AssignmentClass ac = new AssignmentClass();

                ac.ClassId = cl.ClassID;

                //string together course abb + class name
                foreach (Course cr in courseList)
                {
                    if (cr.CourseID == cl.CourseID)
                    {
                        ac.ClassName = cr.CourseAbbr + "/" + cl.ClassName;
                    }
                }

                //check which class has been assigned the assignment
                foreach (Class_Assgn ca in classAssgn)
                {
                    if (ca.AssignmentID == i && ca.ClassID == ac.ClassId)
                    {
                        ac.isSelected = true;
                    }
                }

                //add to list
                assgnClassList.Add(ac);
            }

            //get all the modules
            modList = db.Modules.Where(mod => mod.DeletedAt == null).ToList();

            //set the data for the assignment 
            model.AssgnTitle = assgn.AssgnTitle;
            model.ModuleId = assgn.ModuleCode;

            //get the id of the assignment module to set the dropdown
            foreach (Module m in modList)
            {
                if (m.ModuleCode == model.ModuleId)
                {
                    model.SelectedModuleId = m.ModuleCode;
                }
            }

            model.AssignmentId = assgn.AssignmentID;
            model.Describe = assgn.Describe;
            model.StartDate = assgn.StartDate;
            model.DueDate = assgn.DueDate;
            model.MaxAttempt = assgn.MaxAttempt;
            model.Modules = modList;
            model.ClassList = assgnClassList;
            model.UpdateSolution = false;
            model.IsTestCasePresent = true;

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateAssignment(UpdateAssignmentViewModel uAVM, HttpPostedFileBase solutionsFileUpload, HttpPostedFileBase testCaseUpload, string command)
        {
            //users click the UPDATE button
            if (command.Equals("Update"))
            {
                //user doesnt wants to update solution
                if (uAVM.UpdateSolution == false)
                {
                    if (UpdateAssignmentToDB(uAVM, false, false) == true)
                    {
                        //failed to update assignment 
                        uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                        uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                        TempData["GeneralError"] = "Failed to update assignment to database. Please try again!";
                        return View(uAVM);
                    }
                }

                //user wants to update solution 
                if (uAVM.UpdateSolution == true)
                {
                    //run solution with testcase
                    if (uAVM.IsTestCasePresent == true)
                    {
                        if ((solutionsFileUpload != null && Path.GetExtension(solutionsFileUpload.FileName) == ".zip") && (testCaseUpload != null && Path.GetExtension(testCaseUpload.FileName) == ".xml"))
                        {
                            if (solutionsFileUpload.ContentLength > 0 && testCaseUpload.ContentLength > 0)
                            {
                                if (solutionsFileUpload.ContentLength < 104857600)
                                {
                                    string slnFilePath = "";
                                    string assignmentTitle = (uAVM.AssgnTitle).Replace(" ", ""); //used to name the testcase/solution temporarily until get assignmentID 
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

                                    //get the language and pass into grader
                                    ProgLanguage lang = db.ProgLanguages.ToList().Find(l => l.LanguageId == db.Modules.ToList().Find(m => m.ModuleCode == uAVM.ModuleId).LanguageId);

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

                                    //exit codes returned from grader 
                                    //1 is successfully done everything
                                    //2 is test case submitted could not be read
                                    //3 is program has failed to run
                                    //4 is program was caught in an infinite loop

                                    Sandboxer sandbox = new Sandboxer(slnFilePath, fileName, assignmentTitle, lang.LangageType, true);
                                    int exitCode = (int)sandbox.runSandboxedGrading();

                                    if (exitCode == 1)
                                    {
                                        //update DB + rename solution/testcase
                                        if (UpdateAssignmentToDB(uAVM, true, true) == true)
                                        {
                                            //failed to update DB
                                            DeleteFile(fileName, assignmentTitle, true);
                                            uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                            uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                            TempData["GeneralError"] = "Failed to save assignment to database. Please try again.";
                                            return View(uAVM);
                                        }

                                        //delete the uploaded sln but not test case
                                        DeleteFile(fileName, assignmentTitle, false);

                                    }//end of run succesfully method 

                                    else if (exitCode == 2)
                                    {
                                        DeleteFile(fileName, assignmentTitle, true);
                                        uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                        uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                        TempData["GeneralError"] = "The test case submitted could not be read properly. Please check your test case file.";
                                        return View(uAVM);
                                    }

                                    else if (exitCode == 3)
                                    {
                                        //solution failed to run 
                                        DeleteFile(fileName, assignmentTitle, true);
                                        uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                        uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                        TempData["GeneralError"] = "The program has failed to run entirely. Please check your program.";
                                        return View(uAVM);
                                    }
                                    else if (exitCode == 4)
                                    {
                                        //solution stuck in infinite loop
                                        DeleteFile(fileName, assignmentTitle, true);
                                        uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                        uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                        TempData["GeneralError"] = "The program uploaded was caught in an infinite loop. Please check your program.";
                                        return View(uAVM);
                                    }
                                }
                                else
                                {
                                    //more than 150MB                     
                                    uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                    TempData["SlnWarning"] = "Please make sure that your file is less than 150MB!";
                                    return View(uAVM);
                                }
                            }
                            else
                            {
                                //empty file 
                                uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                string err = "Uploaded file is invalid! Please try again.";
                                TempData["SlnWarning"] = err;
                                TempData["TcWarning"] = err;
                                return View(uAVM);
                            }
                        }
                        else
                        {
                            //uploaded file is invalid
                            uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                            uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                            string err = "Uploaded file is invalid! Please try again.";
                            TempData["SlnWarning"] = err;
                            TempData["TcWarning"] = err;
                            return View(uAVM);
                        }
                    }//end of run with testcase

                    //run solution without testcase 
                    if (uAVM.IsTestCasePresent == false)
                    {
                        if (solutionsFileUpload != null && Path.GetExtension(solutionsFileUpload.FileName) == ".zip")
                        {
                            if (solutionsFileUpload.ContentLength > 0)
                            {
                                if (solutionsFileUpload.ContentLength < 104857600)
                                {
                                    string slnFilePath = "";
                                    string assignmentTitle = (uAVM.AssgnTitle).Replace(" ", "");
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
                                    ProgLanguage lang = db.ProgLanguages.ToList().Find(l => l.LanguageId == db.Modules.ToList().Find(m => m.ModuleCode == uAVM.ModuleId).LanguageId);

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

                                    Sandboxer sandBoxGrading = new Sandboxer(slnFilePath, fileName, assignmentTitle, lang.LangageType, false);
                                    int exitCode = (int)sandBoxGrading.runSandboxedGrading();

                                    if (exitCode == 1)
                                    {
                                        //save to DB + rename the NEW solution file + delete the OLD solution file
                                        if (UpdateAssignmentToDB(uAVM, true, false) == true)
                                        {
                                            //solution has failed to save to DB
                                            DeleteFile(fileName, assignmentTitle, false);
                                            uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                            uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                            TempData["GeneralError"] = "Failed to save assignment to database! Please try again.";
                                            return View(uAVM);
                                        }

                                        //delete the uploaded sln
                                        DeleteFile(fileName, assignmentTitle, false);

                                    }
                                    else if (exitCode == 3)
                                    {
                                        //solution failed to run 
                                        DeleteFile(fileName, assignmentTitle, false);
                                        uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                        uAVM.ClassList = UpdateClassList(uAVM.ClassList); ;
                                        TempData["GeneralError"] = "The program uploaded was caught in an infinite loop. Please check your program.";
                                        return View(uAVM);
                                    }

                                }
                                else
                                {
                                    //file size is more that 150MB
                                    uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                    TempData["SlnWarning"] = "Please make sure that your file is less than 150MB!";
                                    return View(uAVM);
                                }
                            }
                            else
                            {
                                //empty file 
                                uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                                string err = "Uploaded file is invalid! Please try again.";
                                TempData["SlnWarning"] = err;
                                TempData["TcWarning"] = err;
                                return View(uAVM);
                            }
                        }
                        else
                        {
                            //invalid file 
                            uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                            uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                            string err = "Uploaded file is invalid! Please try again.";
                            TempData["SlnWarning"] = err;
                            TempData["TcWarning"] = err;
                            return View(uAVM);
                        }

                    }//end of run without testcase 

                }//end of updateSolution

            }
            else
            {
                //delete assignment 
                if (DeleteAssignment(uAVM) == true)
                {
                    //failed to delete assignment 
                    uAVM.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                    uAVM.ClassList = UpdateClassList(uAVM.ClassList);
                    TempData["GeneralError"] = "Failed to delete assignment. Please try again!";
                    return View(uAVM);
                }
            }

            //successfully updating assignment to DB
            return RedirectToAction("ManageAssignments", "Lecturer");
        }


        public bool DeleteAssignment(UpdateAssignmentViewModel uAVM)
        {
            bool isFailed = false;

            try
            {
                //remove the assignment from the assigned classes 
                db.Class_Assgn.RemoveRange(db.Class_Assgn.Where(ca => ca.AssignmentID == uAVM.AssignmentId));

                //update the assignment deleted status to something 
                Assignment a = db.Assignments.Where(at => at.AssignmentID == uAVM.AssignmentId).FirstOrDefault();
                a.DeletedAt = DateTime.Now;
                a.DeletedBy = User.Identity.GetUserName();
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isFailed = true;
            }

            return isFailed;
        }

        public bool UpdateAssignmentToDB(UpdateAssignmentViewModel uVM, bool updateSln, bool isTestCase)
        {
            Assignment updatedAssignment = new Assignment();
            bool isFailed = false;
            string assignmentTitle = (uVM.AssgnTitle).Replace(" ", "");

            //get the previous asssignment from db
            updatedAssignment = db.Assignments.Where(a => a.AssignmentID == uVM.AssignmentId).FirstOrDefault();

            //update the assignment values 
            try
            {
                //update the values 
                updatedAssignment.AssgnTitle = uVM.AssgnTitle;
                updatedAssignment.ModuleCode = uVM.ModuleId;
                updatedAssignment.Describe = uVM.Describe;
                updatedAssignment.MaxAttempt = uVM.MaxAttempt;
                updatedAssignment.StartDate = uVM.StartDate;
                updatedAssignment.DueDate = uVM.DueDate;
                updatedAssignment.UpdatedBy = User.Identity.GetUserName();
                updatedAssignment.UpdatedAt = DateTime.Now;

                //remove previously assigned classes 
                db.Class_Assgn.RemoveRange(db.Class_Assgn.Where(ca => ca.AssignmentID == uVM.AssignmentId));

                //re-assign the assignments to the classes 
                List<Class_Assgn> newAssgn = new List<Class_Assgn>();
                foreach (AssignmentClass ac in uVM.ClassList)
                {
                    if (ac.isSelected == true)
                    {
                        Class_Assgn a = new Class_Assgn();
                        a.AssignmentID = uVM.AssignmentId;
                        a.ClassID = ac.ClassId;
                        newAssgn.Add(a);
                    }
                }

                db.Class_Assgn.AddRange(newAssgn);

                //runs only IF users want to update their solution
                if (updateSln == true)
                {
                    //delete the old solution that is stored  
                    var solutionPath = Server.MapPath(@"~/Solutions/");
                    var oldSlnName = uVM.AssignmentId + "solution.xml";

                    DirectoryInfo di;
                    if ((di = new DirectoryInfo(solutionPath)).Exists)
                    {
                        foreach (FileInfo f in di.GetFiles())
                        {
                            //find the original solution 
                            if (f.Name == oldSlnName)
                            {
                                //delete it 
                                f.IsReadOnly = false;
                                f.Delete();
                            }
                        }
                    }

                    //now that the old solution xml is gone, we reaname the new solution file 
                    foreach (FileInfo f in new DirectoryInfo(solutionPath).GetFiles())
                    {
                        if (f.Name == assignmentTitle + ".xml")
                        {
                            var sourcePath = solutionPath + f.Name;
                            var destPath = solutionPath + uVM.AssignmentId + "solution.xml";
                            FileInfo info = new FileInfo(sourcePath);
                            info.MoveTo(destPath);
                        }
                    }

                    //this part only runs IF a testcase is present 
                    if (isTestCase == true)
                    {
                        var testCasePath = Server.MapPath(@"~/TestCase/");
                        var oldTestCase = uVM.AssignmentId + "testcase.xml";

                        //delete the old testcase 
                        if ((di = new DirectoryInfo(testCasePath)).Exists)
                        {
                            foreach (FileInfo f in di.GetFiles())
                            {
                                //find the original solution 
                                if (f.Name == oldTestCase)
                                {
                                    //delete it 
                                    f.IsReadOnly = false;
                                    f.Delete();
                                }
                            }
                        }

                        //now that it is deleted, we rename the new testcase uploaded
                        foreach (FileInfo f in new DirectoryInfo(testCasePath).GetFiles())
                        {
                            if (f.Name == assignmentTitle + ".xml")
                            {
                                var sourcePath = testCasePath + f.Name;
                                var destPath = testCasePath + uVM.AssignmentId + "testcase.xml";
                                FileInfo info = new FileInfo(sourcePath);
                                info.MoveTo(destPath);
                            }
                        }

                    }//end of update testcase xml

                }//end of update solution xml

                db.SaveChanges();

            }//end of try 
            catch (Exception ex)
            {
                isFailed = true;
            }

            return isFailed;
        }

        //Add Assignment
        public FileResult DownloadTestCase()
        {
            string f = Server.MapPath(@"~/TestCase/testcase.xml");
            byte[] fileBytes = System.IO.File.ReadAllBytes(f);
            string fileName = "testcase.xml";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult AddAssignment()
        {
            AddAssignmentViewModel aaVM = new AddAssignmentViewModel();

            List<AssignmentClass> ac = new List<AssignmentClass>();
            List<Class> managedClasses = new List<Class>();
            List<Course> courseList = new List<Course>();

            var x = User.Identity.GetUserName();

            //get the classes managed by the lecturer 
            var query = from c in db.Classes join lc in db.Lec_Class on c.ClassID equals lc.ClassID where lc.StaffID.Equals(x) where c.DeletedAt == null select c;
            managedClasses = query.ToList();

            //get all courses 
            courseList = db.Courses.Where(c => c.DeletedAt == null).ToList();

            //we loop through the managedClasses to fill up the assignmentclass -> which is used to populate checkboxes
            foreach (var c in managedClasses)
            {
                AssignmentClass a = new AssignmentClass();

                foreach (Course cr in courseList)
                {
                    if (c.CourseID == cr.CourseID)
                    {
                        //string the course abb + class name together 
                        a.ClassName = cr.CourseAbbr + "/" + c.ClassName;
                    }
                }

                a.ClassId = c.ClassID;
                a.isSelected = false;
                ac.Add(a);
            }

            //get the modules 
            List<Module> allModules = db.Modules.ToList().FindAll(mod => mod.DeletedAt == null);

            aaVM.IsTestCasePresent = true;
            aaVM.ClassList = ac;
            aaVM.Modules = allModules;
            aaVM.IsPostBack = 0;

            return View(aaVM);
        }

        [HttpPost]
        public ActionResult AddAssignment(AddAssignmentViewModel addAssgn, HttpPostedFileBase solutionsFileUpload, HttpPostedFileBase testCaseUpload)
        {
            int exitCode = 0, counter = 0;
            bool isJobRunning;
            string currentJobId;

            //run solution with testcase
            if (addAssgn.IsTestCasePresent == true)
            {
                if ((solutionsFileUpload != null && Path.GetExtension(solutionsFileUpload.FileName) == ".zip") && (testCaseUpload != null && Path.GetExtension(testCaseUpload.FileName) == ".xml"))
                {
                    if (solutionsFileUpload.ContentLength > 0 && testCaseUpload.ContentLength > 0)
                    {
                        if (solutionsFileUpload.ContentLength < 104857600)
                        {
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
                            else if (lang.LangageType.Equals("Python"))
                            {
                                ogPath = slnFilePath + "/" + fileName + ".py";
                                newPath = path + "/" + fileName + ".py";
                            }
                            else
                            {
                                //solution stuck in infinite loop
                                DeleteFile(fileName, assignmentTitle, true);
                                addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                ModelState.Remove("IsPostBack");
                                addAssgn.IsPostBack = 1;
                                TempData["GeneralError"] = "The program uploaded is unsupported by the compiler used for this module. Please upload "
                                    + "program coded in the appropriate programming language or ensure you have selected the correct module.";
                                return View(addAssgn);
                            }

                            System.IO.File.Move(ogPath, newPath);

                            //save the testcase
                            var filePath = Server.MapPath(@"~/TestCase/" + assignmentTitle + ".xml");
                            var fileInfo = new FileInfo(filePath);
                            fileInfo.Directory.Create();
                            testCaseUpload.SaveAs(filePath);

                            //exit codes returned from grader 
                            //1 is successfully done everything
                            //2 is test case submitted could not be read
                            //3 is program has failed to run
                            //4 is program was caught in an infinite loop
                            //5 is Unsupported Language Type

                            //add the running of solution to queue
                            currentJobId = BackgroundJob.Enqueue(() => ProcessSubmission(slnFilePath, fileName, assignmentTitle, lang.LangageType, true));

                            //check with DB if the job has finished running
                            do
                            {
                                isJobRunning = QueryJobFinish(currentJobId);
                                counter++;

                            } while (isJobRunning && counter < 10000);


                            //Finished processing the assignment
                            if (isJobRunning == false)
                            {
                                //this is the result reutrned from after the assignment has been processed
                                if (counter >= 10000)
                                {
                                    //the program was caught in infinite loop and the scheduler cannot process in time
                                    exitCode = 2952;
                                    //we delete the job
                                    BackgroundJob.Delete(currentJobId);
                                }
                                else
                                {
                                    //program was run on time
                                    exitCode = (int)TempData["ExitCode"];
                                }

                                //post back results 
                                if (exitCode == 1)
                                {
                                    //save to DB + rename solution/testcase
                                    if (AddAssignmentToDB(addAssgn, fileName, true) == true)
                                    {
                                        //failed to save to DB
                                        DeleteFile(fileName, assignmentTitle, true);
                                        addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                        addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                        ModelState.Remove("IsPostBack");
                                        addAssgn.IsPostBack = 1;
                                        TempData["GeneralError"] = "Failed to save assignment to database. Please try again.";
                                        return View(addAssgn);
                                    }
                                    //delete the uploaded sln
                                    DeleteFile(fileName, assignmentTitle, false);

                                }//end of run succesfully condition

                                else if (exitCode == 2)
                                {
                                    DeleteFile(fileName, assignmentTitle, true);
                                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                    ModelState.Remove("IsPostBack");
                                    addAssgn.IsPostBack = 1;
                                    TempData["GeneralError"] = "The test case submitted could not be read properly. Please check your test case file.";
                                    return View(addAssgn);

                                }//end of failed testcase condition

                                else if (exitCode == 3)
                                {
                                    //solution failed to run 
                                    DeleteFile(fileName, assignmentTitle, true);
                                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                    ModelState.Remove("IsPostBack");
                                    addAssgn.IsPostBack = 1;
                                    TempData["GeneralError"] = "The program has failed to run entirely. Please check your program";
                                    return View(addAssgn);

                                }//end of program failed to run condition

                                else if (exitCode == 4)
                                {
                                    //solution stuck in infinite loop
                                    DeleteFile(fileName, assignmentTitle, true);
                                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                    ModelState.Remove("IsPostBack");
                                    addAssgn.IsPostBack = 1;
                                    TempData["GeneralError"] = "The program uploaded was caught in an infinite loop. Please check your program.";
                                    return View(addAssgn);

                                }//end of infinite loop condition

                                else if (exitCode == 5)
                                {
                                    //unsupported langugage
                                    DeleteFile(fileName, assignmentTitle, true);
                                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                    ModelState.Remove("IsPostBack");
                                    addAssgn.IsPostBack = 1;
                                    TempData["GeneralError"] = "The program uploaded is unsupported by the compiler used for this module. Please upload "
                                        + "program coded in the appropriate programming language or ensure you have selected the correct module." +
                                        " Support for that language could also not be added yet.";
                                    return View(addAssgn);

                                }//end of unsupported language condition

                                else if (exitCode == 2952)
                                {
                                    //scheduler is taking too long to grade/infinite loop. so we post back to user
                                    DeleteFile(fileName, assignmentTitle, true);
                                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                    ModelState.Remove("IsPostBack");
                                    addAssgn.IsPostBack = 1;
                                    TempData["GeneralError"] = "The program uploaded was caught in an infinite loop and was unable to generate answers on time. Please upload your solution and try again !";
                                    return View(addAssgn);

                                }//end of scheduler failed to run

                            }//end of assignment processing
                        }
                        else
                        {
                            //uploaded file is more than 150MB
                            addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                            addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                            ModelState.Remove("IsPostBack");
                            addAssgn.IsPostBack = 1;
                            TempData["SlnWarning"] = "Please make sure that your file is less than 150MB!";
                            return View(addAssgn);

                        }//end of file too big condition
                    }
                    else
                    {
                        //uploaded file is empty 
                        addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                        addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                        ModelState.Remove("IsPostBack");
                        addAssgn.IsPostBack = 1;
                        string err = "Uploaded file is invalid! Please try again.";
                        TempData["SlnWarning"] = err;
                        TempData["TcWarning"] = err;
                        return View(addAssgn);

                    }//end of empty file condition
                }
                else
                {
                    //uploaded file is invalid
                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                    ModelState.Remove("IsPostBack");
                    addAssgn.IsPostBack = 1;
                    string err = "Uploaded file is invalid! Please try again.";
                    TempData["SlnWarning"] = err;
                    TempData["TcWarning"] = err;
                    return View(addAssgn);

                }//end of invalid file condition

            }//end of run with testcase

            //run without testcase 
            else if (addAssgn.IsTestCasePresent == false)
            {
                if (solutionsFileUpload != null && Path.GetExtension(solutionsFileUpload.FileName) == ".zip")
                {
                    if (solutionsFileUpload.ContentLength > 0)
                    {
                        if (solutionsFileUpload.ContentLength < 104857600)
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

                            //add the grading to queue
                            currentJobId = BackgroundJob.Enqueue(() => ProcessSubmission(slnFilePath, fileName, assignmentTitle, lang.LangageType, false));

                            //check if the job has finished processing
                            do
                            {
                                isJobRunning = QueryJobFinish(currentJobId);
                                counter++;

                            } while (isJobRunning && counter < 10000);

                            //job has finishd processing
                            if (isJobRunning == false)
                            {
                                //get the exit code which is generated from the processing of the assignment
                                if (counter >= 10000)
                                {
                                    //the program was caught in infinite loop and the scheduler cannot process in time
                                    exitCode = 2952;
                                    //we delete the job
                                    BackgroundJob.Delete(currentJobId);
                                }
                                else
                                {
                                    //program was run on time
                                    exitCode = (int)TempData["ExitCode"];
                                }

                                //post back result
                                if (exitCode == 1)
                                {
                                    //save to DB + rename solution file
                                    if (AddAssignmentToDB(addAssgn, fileName, false) == true)
                                    {
                                        //solution has failed to save to DB
                                        DeleteFile(fileName, assignmentTitle, false);
                                        addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                        addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                        ModelState.Remove("IsPostBack");
                                        addAssgn.IsPostBack = 1;
                                        TempData["GeneralError"] = "Failed to save assignment to database! Please try again.";
                                        return View(addAssgn);
                                    }

                                    //delete the uploaded sln
                                    DeleteFile(fileName, assignmentTitle, false);

                                }
                                else if (exitCode == 3)
                                {
                                    //solution failed to run 
                                    DeleteFile(fileName, assignmentTitle, false);
                                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                    ModelState.Remove("IsPostBack");
                                    addAssgn.IsPostBack = 1;
                                    TempData["GeneralError"] = "The program uploaded was caught in an infinite loop. Please check your program.";
                                    return View(addAssgn);
                                }

                                else if (exitCode == 2952)
                                {
                                    //scheduler is taking too long to grade/infinite loop. so we post back to user
                                    DeleteFile(fileName, assignmentTitle, false);
                                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                                    ModelState.Remove("IsPostBack");
                                    addAssgn.IsPostBack = 1;
                                    TempData["GeneralError"] = "The program uploaded was caught in an infinite loop and was unable to generate answers on time. Please upload your solution and try again !";
                                    return View(addAssgn);

                                }//end of scheduler failed to run

                            }//end of solution processing
                        }
                        else
                        {
                            //uploaded file is more than 150MB
                            addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                            addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                            ModelState.Remove("IsPostBack");
                            addAssgn.IsPostBack = 1;
                            TempData["SlnWarning"] = "Please make sure that your file is less than 150MB!";
                            return View(addAssgn);

                        }//end of file too big condition
                    }
                    else
                    {
                        //uploaded file is empty 
                        addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                        addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                        ModelState.Remove("IsPostBack");
                        addAssgn.IsPostBack = 1;
                        TempData["SlnWarning"] = "Uploaded file is empty! Please try again.";
                        return View(addAssgn);

                    }//end of empty file condition (seriously?)
                }
                else
                {
                    //uploaded file is invalid 
                    addAssgn.Modules = db.Modules.Where(m => m.DeletedAt == null).ToList();
                    addAssgn.ClassList = UpdateClassList(addAssgn.ClassList);
                    ModelState.Remove("IsPostBack");
                    addAssgn.IsPostBack = 1;
                    TempData["SlnWarning"] = "Uploaded file is invalid! Please try again.";
                    return View(addAssgn);

                }//end of invalid file upload function

            }//end of run without testcase

            //everything all okay 
            return RedirectToAction("ManageAssignments", "Lecturer");
        }

        //for Hangfire to run the submission
        public int ProcessSubmission(string slnFilePath, string fileName, string assignmentTitle, string langType, bool isTestCasePresent)
        {
            int exitCode = 0;

            //run the assignment grading in scheduler
            Sandboxer sandbox = new Sandboxer(slnFilePath, fileName, assignmentTitle, langType, isTestCasePresent);
            exitCode = (int)sandbox.runSandboxedGrading();

            return exitCode;
        }

        //check if the current JOB has finished running
        public bool QueryJobFinish(string jobId)
        {
            //this method is to check the DB IF the job is finished
            bool runningJob = true;
            int runningJobId = Int32.Parse(jobId);
            //this is the row with all the necessary data we need
            State finalState = new State();

            //we check the Hangfire.State table to see if our current job has succeeded running 
            var stateList = db.States.Where(s => s.JobId == runningJobId).ToList();

            //why stateList must be 3 then it will find the data
            //Hangfire stores 3 job states -> Enqueued, Processing, Succeeded/Failure (found in Hangfire.State in DB)
            //the return value is stored in the last state, which is either succeeded or failed
            //to reduce workload on the server when querying job status, thats why it only goes into the loop when it has 'completed' the job
            if (stateList.Count() == 3)
            {
                foreach (State s in stateList)
                {
                    if (s.Name == "Succeeded")
                    {
                        runningJob = false;
                        finalState = s;
                    }
                    if (s.Name == "Failed")
                    {
                        //job has failed. break away from looping
                        runningJob = false;
                        finalState = s;

                        //also, stop the job immediately as hangfire WILL re-try
                        BackgroundJob.Delete(jobId);
                    }
                }
            }

            //IF the job has been run, we get the result and add it togther with the earlier submission model we partially filled up
            if (runningJob == false)
            {
                //Hangfire stores the data as JSON. We deserialize it here to a DataObj -> which is shaped like JSON structure
                HangfireData dataObj = JsonConvert.DeserializeObject<HangfireData>(finalState.Data);

                //if the job was completed succeessfully -> means the work got graded
                if (finalState.Name == "Succeeded")
                {
                    TempData["ExitCode"] = (int)dataObj.Result;
                }

                //if the job failed -> means the work didnt get graded and/or ran into some errors somewhere somehow
                if (finalState.Name == "Failed")
                {
                    TempData["ExitCode"] = 3;
                }
            }

            //this is to indicate if the job has finished running or not
            return runningJob;
        }

        public bool AddAssignmentToDB(AddAssignmentViewModel addAssgn, string fileName, bool isTestCase)
        {
            //now to add into the DB
            Assignment newAssignment = new Assignment();
            Class_Assgn classAssgn = new Class_Assgn();
            List<HttpPostedFileBase> assgnFiles = new List<HttpPostedFileBase>();
            string assignmentTitle = (addAssgn.AssgnTitle).Replace(" ", "");
            bool isFailed = false; //used to tell the user if the assignment has already been successfully save to DB

            try
            {
                //save the main assignment to DB
                addAssgn.Solution = "~/Solutions/" + assignmentTitle + ".xml";
                newAssignment.AssgnTitle = addAssgn.AssgnTitle;
                newAssignment.Describe = addAssgn.Describe;
                newAssignment.MaxAttempt = addAssgn.MaxAttempt;
                newAssignment.StartDate = addAssgn.StartDate;
                newAssignment.DueDate = addAssgn.DueDate;
                newAssignment.Solution = addAssgn.Solution;
                newAssignment.ModuleCode = addAssgn.ModuleId;
                newAssignment.CreateBy = User.Identity.GetUserName();
                newAssignment.CreateAt = DateTime.Now;
                newAssignment.UpdatedBy = User.Identity.GetUserName();
                newAssignment.UpdatedAt = DateTime.Now;
                db.Assignments.Add(newAssignment);
                db.SaveChanges();

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
                        db.SaveChanges();
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

            }//end of try 

            catch (Exception ex)
            {
                //failed to save to DB. will show something to user
                isFailed = true;
            }
            return isFailed;
        }

        //Common methods between ADD and UPDATE assignment
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

        public List<AssignmentClass> UpdateClassList(List<AssignmentClass> ClassList)
        {
            //classes the lectuer manages 
            List<Class> managedClasses = new List<Class>();

            //courses (to string together the name)
            List<Course> courseList = new List<Course>();

            var x = User.Identity.GetUserName();

            //get the classes the lecturer manages again 
            var query = from c in db.Classes join lc in db.Lec_Class on c.ClassID equals lc.ClassID where lc.StaffID.Equals(x) where c.DeletedAt == null select c;
            managedClasses = query.ToList();

            //get all the courses 
            courseList = db.Courses.ToList();

            foreach (AssignmentClass ac in ClassList)
            {
                //check the class NAME
                foreach (Class c in managedClasses)
                {
                    if (c.ClassID == ac.ClassId)
                    {
                        //check the class COURSE
                        foreach (Course cr in courseList)
                        {
                            if (cr.CourseID == c.CourseID)
                            {
                                //string the className + courseName together
                                ac.ClassName = cr.CourseAbbr + "/" + c.ClassName;
                            }

                        }//end of course loop

                    }//end of class loop
                }
            }//end of assignment class loop

            //now we return the updated ClassList
            return ClassList;
        }

        //View Results
        public ActionResult ViewResults()
        {
            ViewResultsViewModel vrvm = new ViewResultsViewModel();
            string loggedInLecturer = User.Identity.GetUserName();

            List<Class> managedClasses = new List<Class>();

            List<Lec_Class> lec_classes = db.Lec_Class.Where(lc => lc.StaffID == loggedInLecturer).ToList();

            foreach (Lec_Class lc in lec_classes)
            {
                List<Class> temp = db.Classes.Where(c => c.DeletedAt == null).Where(c => c.ClassID == lc.ClassID).ToList();
                managedClasses.AddRange(temp);
            }


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
            string loggedInLecturer = User.Identity.GetUserName();

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

        public ActionResult ViewTestcase(string assignmentId)
        {
            XmlDocument testCaseFile = new XmlDocument();
            var pathToTestcase = Server.MapPath(@"~/TestCase/" + assignmentId + "testcase.xml");
            testCaseFile.Load(pathToTestcase);
            List<TestCase> tc = new List<TestCase>();

            ViewTestCaseViewModel vtcvm = new ViewTestCaseViewModel();

            //vtcvm.testCaseFile = testCaseFile;

            XmlNodeList testcaseList = testCaseFile.SelectNodes("/body/testcase");
            foreach (XmlNode node in testcaseList)
            {
                List<string> inputs = new List<string>();
                TestCase testcase = new TestCase();

                foreach (XmlNode input in node.ChildNodes)
                {
                    inputs.Add(input.InnerText);
                }
                testcase.inputs = inputs;
                tc.Add(testcase);
            }
            vtcvm.testcases = tc;

            return View(vtcvm);
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