using SPade.Models;
using SPade.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Models.DAL;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using Microsoft.AspNet.Identity;

namespace SPade.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        //  { UserID = Request.QueryString["UserID"]

        private SPadeDBEntities db = new SPadeDBEntities();

        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult BulkAddLecturer()
        {
            return View();
        }

        public ActionResult BulkAddStudent()
        {
            return View();
        }

        public ActionResult AddModule()
        {
            List<ProgLanguage> languageList = new List<ProgLanguage>();
            languageList = db.ProgLanguages.ToList();
            AddModuleViewModel mVM = new AddModuleViewModel();
            mVM.Languages = languageList;

            return View(mVM);
        }

        [HttpPost]
        public ActionResult AddModule(AddModuleViewModel addModuleVM)
        {
            Module module = new Module();

            try
            {
                module.ModuleCode = addModuleVM.ModuleCode;
                module.ModuleName = addModuleVM.ModuleName;
                module.LanguageId = addModuleVM.ProgLangId;
                module.CreatedAt = DateTime.Now;
                module.CreatedBy = User.Identity.GetUserName();
                module.UpdatedAt = DateTime.Now;
                module.UpdatedBy = User.Identity.GetUserName();
                db.Modules.Add(module);

                db.SaveChanges();

            }
            catch (Exception ex)
            {
                addModuleVM.Languages = db.ProgLanguages.ToList();
                TempData["Error"] = "Failed to save module. Please try again !";
                return View(addModuleVM);
            }

            return RedirectToAction("Dashboard", "ManageModule");
        }
        [HttpPost]
        public ActionResult AddOneStudent(AddStudentViewModel model)
        {
            try
            {
                var student = new Student()
                {
                    AdminNo = model.AdminNo.Trim(),
                    Name = model.Name,
                    Email = model.Email,
                    ContactNo = model.ContactNo,
                    ClassID = '1',
                    CreatedBy = "Admin",
                    UpdatedBy = "Admin",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.Students.Add(student);
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

            }
            return View(model);

        }
        public ActionResult AddOneStudent()
        {

            AddStudentViewModel model = new AddStudentViewModel();
            //Get all classes
            List<Class> allClasses = db.Classes.ToList();
            model.Classes = allClasses;
            return View(model);



        }
        [HttpPost]
        public ActionResult AddOneClass(AddClassViewModel model)
        {

            //Get all classes
            List<Course> allCourses = db.Courses.ToList();
            model.Courses = allCourses;
            List<Lecturer> allLecturer = db.Lecturers.ToList();
            model.Lecturers = allLecturer;

            try
            {
                var class1 = new Class()
                {
                    ClassName = model.ClassName,
                    CourseID = model.CourseID,
                    CreatedBy = "Admin",
                    UpdatedBy = "Admin",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,

                };
                var lec_class = new Lec_Class()
                {
                    ClassID = model.ClassID,
                    StaffID = model.StaffID,

                };


                db.Classes.Add(class1);
                db.Lec_Class.Add(lec_class);
                db.SaveChanges();

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

            }
            return View(model);
        }
        public ActionResult AddOneClass()
        {
            AddClassViewModel model = new AddClassViewModel();
            //Get all classes
            List<Course> allCourses = db.Courses.ToList();
            model.Courses = allCourses;
            List<Lecturer> allLecturer = db.Lecturers.ToList();
            model.Lecturers = allLecturer;
            return View(model);

        }
        [HttpPost]
        public ActionResult AddOneLecturer(AddLecturerViewMode model)
        {
            try
            {
                var lecturer = new Lecturer()
                {
                    StaffID = model.StaffID,
                    Name = model.Name,
                    ContactNo = model.ContactNo,
                    Email = model.Email,
                    CreatedBy = "Admin",
                    UpdatedBy = "Admin",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,

                };

                db.Lecturers.Add(lecturer);
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

            }
            return View(model);
        }
        public ActionResult AddOneLecturer()
        {
            return View();
        }
        public ActionResult ManageClass()
        {
            List<Lecturer> lecturer = new List<Lecturer>();
            List<Class> classes = new List<Class>();
            List<Lec_Class> lc = db.Lec_Class.ToList().FindAll(c => c.ClassID == 1);


            foreach (Lec_Class i in lc)
            {
                lecturer = db.Lecturers.ToList().FindAll(lect => lect.StaffID == i.StaffID);
            }






            return View();
        }
        public ActionResult ManageStudent()
        {
            ManageStudentViewModel ms = new ManageStudentViewModel();

            List<Student> students = new List<Student>();

            students = db.Students.ToList();

            return View(students);

        }
        public ActionResult ManageLecturer()
        {
            ManageLecturerViewModel ml = new ManageLecturerViewModel();
            List<Lecturer> lecturer = new List<Lecturer>();
            lecturer = db.Lecturers.ToList();
            return View();
        }
        public ActionResult UpdateClass()
        {
            UpdateClassViewModel model = new UpdateClassViewModel();
            int x = 3;
            //Get all courses
            List<Course> allCourses = db.Courses.ToList();
            model.Courses = allCourses;

            //Get all lecturer
            List<Lecturer> allLecturer = db.Lecturers.ToList();
            model.Lecturers = allLecturer;

            List<Lec_Class> all_Lec_Class = db.Lec_Class.ToList();
            model.Lec_Classes = all_Lec_Class;


            //Get Class           
            List<Class> Classes = db.Classes.ToList();

            foreach (Class C in Classes)
            {
                if (C.ClassID.Equals(x))
                {
                    model.CourseID = C.CourseID;
                    model.ClassID = C.ClassID;
                    model.ClassName = C.ClassName;
                }

            }
            foreach (Lec_Class LC in all_Lec_Class)
            {
                if (LC.ClassID.Equals(x))
                {
                    model.StaffID = LC.StaffID;
                }

            }


            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateClass(UpdateClassViewModel model, string command)
        {
            int x = 3;


            //Get all courses
            List<Course> allCourses = db.Courses.ToList();
            model.Courses = allCourses;

            //Get all lecturer
            List<Lecturer> allLecturer = db.Lecturers.ToList();
            model.Lecturers = allLecturer;

            //Get all lec_class
            List<Lec_Class> all_Lec_Class = db.Lec_Class.ToList();
            model.Lec_Classes = all_Lec_Class;

            //Get Class           
            List<Class> Classes = db.Classes.ToList();

            if (command.Equals("Update"))
            {
                foreach (Class C in Classes)
                {
                    if (C.ClassID.Equals(x))
                    {
                        C.UpdatedBy = "ADMIN";
                        C.UpdatedAt = DateTime.Now;
                        try
                        {
                            TryUpdateModel(C, "", new string[] { "CourseID", "ClassName", "UpdatedBy", "UpdatedAt" });
                            db.SaveChanges();
                        }
                        catch (DataException /* dex */)
                        {
                            //Log the error (uncomment dex variable name and add a line here to write a log.
                            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                            TempData["msg"] = "<script>alert('Updated unsuccessful');</script>";
                        }
                    };
                }
                foreach (Lec_Class LC in all_Lec_Class)
                {
                    if (LC.ClassID.Equals(x))
                    {
                        try
                        {
                            TryUpdateModel(LC, "", new string[] { "StaffID", "ClassID" });
                            db.SaveChanges();
                            //Show alert
                            TempData["msg"] = "<script>alert('Updated successfully');</script>";
                        }
                        catch (DataException /* dex */)
                        {
                            //Log the error (uncomment dex variable name and add a line here to write a log.
                            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                            TempData["msg"] = "<script>alert('Updated unsuccessful');</script>";
                        }
                    };
                }
            }
            else
            {
                foreach (Class C in Classes)
                {
                    if (C.ClassID.Equals(x))
                    {
                        C.DeletedBy = "ADMIN";
                        C.DeletedAt = DateTime.Now;

                        try
                        {
                            TryUpdateModel(C, "", new string[] { "DeletedBy", "DeletedAt" });
                            db.SaveChanges();
                            TempData["msg"] = "<script>alert('Deleted successfully');</script>";
                        }
                        catch (DataException /* dex */)
                        {
                            //Log the error (uncomment dex variable name and add a line here to write a log.
                            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                            TempData["msg"] = "<script>alert('Unable to delete successfully');</script>";
                        }

                    };

                }




            }




            return View(model);




        }
        [HttpGet]
        public ActionResult UpdateStudent()
        {
            UpdateStudentViewModel model = new UpdateStudentViewModel();
            string x = Request.QueryString["AdminNo"];
            //Get all classes
            List<Class> allClasses = db.Classes.ToList();
            model.Classes = allClasses;

            //Get Student           
            List<Student> Students = db.Students.ToList();

            foreach (Student S in Students)
            {
                if (S.AdminNo == x)
                {
                    model.Name = S.Name;
                    model.ClassID = S.ClassID;
                    model.ContactNo = S.ContactNo;
                    model.Email = S.Email;
                }

            }
            return View(model);



        }
        [HttpPost]
        public ActionResult UpdateStudent(UpdateStudentViewModel model, string command)
        {
            string x = "p3333333";
            //Get all classes
            List<Class> allClasses = db.Classes.ToList();
            model.Classes = allClasses;

            //Get Student           
            List<Student> Students = db.Students.ToList();
            //Udate Student information
            if (command.Equals("Update"))
            {
                foreach (Student S in Students)
                {
                    if (S.AdminNo == x)
                    {
                        S.UpdatedBy = "ADMIN";
                        S.UpdatedAt = DateTime.Now;

                        if (TryUpdateModel(S, "",
                           new string[] { "Name", "ClassID", "Email", "ContactNo", "UpdatedAt", "UpdatedBy" }))
                        {
                            try
                            {

                                db.SaveChanges();
                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                            }
                        }
                    }

                }
            }
            //Delete Student
            else
            {
                foreach (Student S in Students)
                {
                    if (S.AdminNo == x)
                    {
                        S.DeletedBy = "ADMIN";
                        S.DeletedAt = DateTime.Now;

                        if (TryUpdateModel(S, "",
                           new string[] { "DeletedBy", "DeletedAt" }))
                        {
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                            }
                        }
                    }

                }
            }





            return View(model);
        }
        public ActionResult UpdateLecturer()
        {
            UpdateLecturerViewModel model = new UpdateLecturerViewModel();
            //Get Lecturer

            string x = "s4444444";
            List<Lecturer> Lecturers = db.Lecturers.ToList();

            foreach (Lecturer L in Lecturers)
            {
                if (L.StaffID == x)
                {
                    model.Name = L.Name;
                    model.ContactNo = L.ContactNo;
                    model.Email = L.Email;

                }

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateLecturer(UpdateLecturerViewModel model, string command)
        {
            string x = "s4444444";
            List<Lecturer> Lecturers = db.Lecturers.ToList();
            if (command.Equals("Update"))
            {
                foreach (Lecturer L in Lecturers)
                {
                    if (L.StaffID == x)
                    {
                        //Update Lecturer
                        if (TryUpdateModel(L, "",
                           new string[] { "Name", "Email", "ContactNo" }))
                        {
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                            }
                        }

                    }

                }
            }
            else
            {

                foreach (Lecturer L in Lecturers)
                {
                    if (L.StaffID == x)
                    {
                        //Update Lecturer
                        L.DeletedBy = "ADMIN";
                        L.DeletedAt = DateTime.Now;

                        if (TryUpdateModel(L, "",
                           new string[] { "DeletedBy", "DeletedAt" }))
                        {
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                            }
                        }
                    }
                }
            }

            return View(model);
        }
        public ActionResult Purge()
        {
            PurgeViewModel pvm = new PurgeViewModel();

            pvm.allAssignments = db.Assignments.ToList();
            pvm.allSubmission = db.Submissions.ToList();
            pvm.allClasses = db.Classes.ToList();
            pvm.classAssgnRel = db.Class_Assgn.ToList();

            return View(pvm);
        }
        [HttpPost]
        public ActionResult Purge(PurgeViewModel pvm, FormCollection formCollection)
        {
            bool subCulled = false;

            if (formCollection["assgnSelected"] != null)
            {
                string[] input = formCollection["assgnSelected"].Split(',');
                foreach (string s in input)
                {
                    int assgnId = Int32.Parse(s);

                    //each string is an id of assignment to be deleted
                    //to purge assignment solutions and testcase
                    //purge from db including relationship with classes

                    //purge solutions
                    System.IO.DirectoryInfo fileDirectory = new DirectoryInfo(Server.MapPath(@"~/Solutions/"));
                    if (fileDirectory.Exists)
                    {
                        foreach (FileInfo files in fileDirectory.GetFiles())
                        {
                            if (files.Name.Replace("solution.xml", "").Equals(s))
                            {
                                files.Delete();//delete all files in directory
                            }
                        }
                    }//end of removing solutions

                    //purge testcase
                    fileDirectory = new DirectoryInfo(Server.MapPath(@"~/TestCase/"));
                    if (fileDirectory.Exists)
                    {
                        foreach (FileInfo files in fileDirectory.GetFiles())
                        {
                            if (files.Name.Replace("testcase.xml", "").Equals(s))
                            {
                                files.Delete();//delete all files in directory
                            }
                        }
                    }//end of removing testcases

                    //remove assignments from db
                    List<Class_Assgn> caToCull = db.Class_Assgn.ToList().FindAll(ca => ca.AssignmentID == assgnId);
                    foreach (Class_Assgn ca in caToCull)
                    {
                        db.Class_Assgn.Remove(ca);
                    }

                    //cullsubmissions first
                    List<Submission> subToCull = db.Submissions.ToList().FindAll(sub => sub.AssignmentID == assgnId);
                    foreach (Submission sub in subToCull)
                    {
                        //purge submissions folder
                        fileDirectory = new DirectoryInfo(Server.MapPath(@"~/Submissions/" + sub.FilePath));

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
                        }//end of removing submissions
                        fileDirectory.Delete(true);
                        db.Submissions.Remove(sub);
                        subCulled = true;
                    }

                    //cull assignment
                    List<Assignment> assgnToCull = db.Assignments.ToList().FindAll(a => a.AssignmentID == assgnId);
                    foreach (Assignment a in assgnToCull)
                    {
                        db.Assignments.Remove(a);
                    }

                    db.SaveChanges();

                }//end of foreach
            }
            else if (formCollection["subSelected"] != null && subCulled == false)
            {
                string[] subInput = formCollection["subSelected"].Split(',');
                foreach (string s in subInput)
                {
                    int subId = Int32.Parse(s);

                    //each string is submissions to be deleted
                    //System.IO.File.AppendAllText("C:/Users/tongliang/Desktop/testSubOutput.txt", s + "\n");

                    Submission sub = db.Submissions.ToList().Find(su => su.SubmissionID == subId);

                    //purge submissions folder
                    System.IO.DirectoryInfo fileDirectory = new DirectoryInfo(Server.MapPath(@"~/Submissions/" + sub.FilePath));

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
                        fileDirectory.Delete(true);
                    }//end of removing submissions

                    //remove from db
                    db.Submissions.Remove(sub);
                    db.SaveChanges();
                }//end of foreach
            }

            return RedirectToAction("Purge");
        }//end of purge controller method
        public ActionResult AddOneAdmin()
        {
            return View();
        }
    }
}