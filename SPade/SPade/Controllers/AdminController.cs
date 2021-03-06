﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SPade.Models;
using SPade.Models.DAL;
using SPade.ViewModels.Admin;
using SPade.ViewModels.Lecturer;
using SPade.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPade.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private SPadeDBEntities db = new SPadeDBEntities();

        public AdminController()
        {

        }

        public AdminController(ApplicationUserManager userManager)
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

        //Manage + Add + Update/Delete Classes 
        public ActionResult ManageClass()
        {
            List<ManageClassViewModel> lvm = new List<ManageClassViewModel>();

            List<Class> x = new List<Class>();
            x = db.Classes.Where(a => a.DeletedAt == null).ToList();

            foreach (Class i in x)
            {
                ManageClassViewModel vm = new ManageClassViewModel();

                vm.ClassID = i.ClassID.ToString();
                vm.Course = db.Courses.ToList().Find(cs => cs.CourseID == i.CourseID).CourseName;
                vm.Class = i.ClassName.ToString();
                vm.CreatedBy = i.CreatedBy.ToUpper();
                vm.NumLecturers = db.Lec_Class.Where(cl => cl.ClassID == i.ClassID).Count().ToString();
                vm.NumStudents = db.Students.Where(cl => cl.ClassID == i.ClassID).Count().ToString();

                lvm.Add(vm);
            }

            return View(lvm);
        }

        public ActionResult AddOneClass()
        {
            AddClassViewModel model = new AddClassViewModel();
            //Get all classes
            List<Course> allCourses = db.Courses.Where(c => c.DeletedAt == null).ToList();
            model.Courses = allCourses;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddOneClass(ViewModels.Admin.AddClassViewModel model, FormCollection formCollection)
        {
            try
            {
                var class1 = new Class()
                {
                    ClassName = model.ClassName,
                    CourseID = int.Parse(formCollection["CourseID"].ToString()),
                    CreatedBy = User.Identity.Name,
                    UpdatedBy = User.Identity.Name,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.Classes.Add(class1);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                //if fail
                TempData["Error"] = "Failed to save class to database. Pease try again!";
                return View(model);
            }

            //if successful 
            return RedirectToAction("ManageClass", "Admin");
        }

        public ActionResult UpdateClass(string id)
        {
            ViewModels.Admin.UpdateClassViewModel model = new ViewModels.Admin.UpdateClassViewModel();

            //Get all courses
            List<Course> allCourses = db.Courses.ToList();
            model.Courses = allCourses;

            //Get class
            Class c = db.Classes.ToList().Find(cl => cl.ClassID == int.Parse(id) && cl.DeletedAt == null);

            model.ClassName = c.ClassName;
            model.ClassID = c.ClassID;
            model.CourseID = c.CourseID;
            model.Courses = allCourses;
            return View(model);

        }

        [HttpPost]
        public ActionResult UpdateClass(ViewModels.Admin.UpdateClassViewModel model, string command, string id)
        {
            List<Course> allCourses = db.Courses.ToList();
            model.Courses = allCourses;
            int classId = int.Parse(id);
            Class c = db.Classes.Where(cl => cl.ClassID == classId).FirstOrDefault();

            //Update Class information
            if (command.Equals("Update"))
            {
                if (classId == 0)
                {
                    ModelState.AddModelError("", "This class holds all students that are not assigned to any class and cannot be updated.");
                    return View(model);
                }
                else
                {
                    c.ClassID = model.ClassID;
                    c.ClassName = model.ClassName;
                    c.CourseID = model.CourseID;
                    c.UpdatedAt = DateTime.Now;
                    c.UpdatedBy = User.Identity.Name;
                    db.SaveChanges();
                    return RedirectToAction("ManageClass");
                }
            }
            //Delete Class
            else
            {
                if (classId == 0)
                {
                    ModelState.AddModelError("", "This class holds all students that are not assigned to any class and cannot be deleted.");
                    return View(model);
                }
                else
                {
                    c.DeletedAt = DateTime.Now;
                    c.DeletedBy = User.Identity.Name;

                    //remove class from lec_class and class_assgn relationship
                    //set student classid to 0
                    List<Lec_Class> lecclass = db.Lec_Class.ToList().FindAll(lc => lc.ClassID == classId);
                    foreach(Lec_Class lecc in lecclass)
                    {
                        db.Lec_Class.Remove(lecc);
                    }

                    List<Class_Assgn> classassgn = db.Class_Assgn.ToList().FindAll(ca => ca.ClassID == classId);
                    foreach(Class_Assgn ca in classassgn)
                    {
                        db.Class_Assgn.Remove(ca);
                    }
                    List<Student> studs = db.Students.ToList().FindAll(s => s.ClassID == classId && s.DeletedAt == null);

                    foreach (Student stud in studs)
                    {
                        stud.ClassID = 0;
                    }

                    db.SaveChanges();
                    return RedirectToAction("ManageClass");
                }
            }
        }

        //Common method for BulkUploadStudent/BulkUploadLecturer
        public void DeleteCSV(string fileName)
        {
            //this method is used to delete the CSV file after it has run successfully 

            var filePath = Server.MapPath(@"~/TempStudentCSV/");
            DirectoryInfo di;

            if ((di = new DirectoryInfo(filePath)).Exists)
            {
                foreach (FileInfo f in di.GetFiles())
                {
                    if (f.Name == fileName)
                    {
                        f.IsReadOnly = false;
                        f.Delete();
                    }
                }
            }

            filePath = Server.MapPath(@"~/TempLecturerCSV/");

            if ((di = new DirectoryInfo(filePath)).Exists)
            {
                foreach (FileInfo f in di.GetFiles())
                {
                    if (f.Name == fileName)
                    {
                        f.IsReadOnly = false;
                        f.Delete();
                    }
                }
            }
        }

        //Manage + Add/Bulk Add + Update/Delete Lecturers 
        public ActionResult ManageLecturer()
        {
            List<ManageLecturerViewModel> lvm = new List<ManageLecturerViewModel>();

            List<Lecturer> x = new List<Lecturer>();
            x = db.Lecturers.Where(a => a.DeletedAt == null).ToList();

            foreach (Lecturer i in x)
            {
                ManageLecturerViewModel vm = new ManageLecturerViewModel();

                vm.StaffID = i.StaffID.ToUpper();
                vm.Name = i.Name;
                vm.ContactNo = i.ContactNo.ToString();
                vm.Email = i.Email;
                vm.CreatedBy = i.CreatedBy.ToUpper();
                vm.NumClasses = db.Lec_Class.Where(cl => cl.StaffID == i.StaffID).Count().ToString();

                lvm.Add(vm);
            }

            return View(lvm);
        }

        public ActionResult AddOneLecturer()
        {
            AddLecturerViewModel model = new AddLecturerViewModel();
            List<Class> allClasses = db.Classes.ToList().FindAll(c => c.DeletedAt == null);
            List<AssignmentClass> ac = new List<AssignmentClass>();

            foreach (var c in allClasses)
            {
                AssignmentClass a = new AssignmentClass();

                String courseAbbr = db.Courses.Where(courses => courses.CourseID == c.CourseID).FirstOrDefault().CourseAbbr;
                String className = courseAbbr + "/" + c.ClassName;

                a.ClassName = className;

                a.ClassId = c.ClassID;
                a.isSelected = false;
                ac.Add(a);
            }
            model.ClassList = ac;
            model.Classes = allClasses;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddOneLecturer(AddLecturerViewModel model, FormCollection formCollection)
        {
            try
            {
                var user = new ApplicationUser { UserName = model.StaffID, Email = model.Email };
                user.EmailConfirmed = true;
                var result = await UserManager.CreateAsync(user, "P@ssw0rd"); //default password
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Lecturer");
                    var lecturer = new Lecturer()
                    {
                        StaffID = model.StaffID,
                        Name = model.Name,
                        ContactNo = model.ContactNo,
                        Email = model.Email,
                        CreatedBy = User.Identity.Name,
                        UpdatedBy = User.Identity.Name,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    foreach (AssignmentClass ac in model.ClassList)
                    {
                        if (ac.isSelected == true)
                        {
                            db.Lec_Class.Add(new Lec_Class
                            {
                                ClassID = ac.ClassId,
                                StaffID = model.StaffID
                            });
                        }
                    }
                    db.Lecturers.Add(lecturer);
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
            return RedirectToAction("ManageLecturer");
        }

        public ActionResult BulkAddLecturer()
        {
            return View();
        }

        public FileResult DownloadBulkAddLecturerFile()
        {
            string f = Server.MapPath(@"~/BulkUploadFiles/BulkAddLecturer.csv");
            byte[] fileBytes = System.IO.File.ReadAllBytes(f);
            string fileName = "BulkAddLecturer.csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public async Task<ActionResult> BulkAddLecturer(HttpPostedFileBase file)
        {
            if ((file != null && Path.GetExtension(file.FileName) == ".csv") && (file.ContentLength > 0))
            {
                //Upload and save the file
                DirectoryInfo di;
                int subNum = 0;
                //check how many files are inside, get the amount 
                if ((di = new DirectoryInfo(Server.MapPath(@"~/TempLecturerCSV/"))).Exists)
                {
                    subNum = di.GetFiles().Count();
                }
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                fileName = fileName + subNum + ".csv";
                //store the file inside TempCSV
                //we name the file as fileName + number of files + 1 to give it an unique identifier
                var path = Path.Combine(Server.MapPath(@"~/TempLecturerCSV/"), fileName);
                file.SaveAs(path);

                string[] lines = System.IO.File.ReadAllLines(path);
                List<Lecturer> lectlist = new List<Lecturer>();

                try
                {
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            Lecturer lect = new Lecturer();
                            lect.StaffID = lines[i].Split(',')[0];
                            lect.Name = lines[i].Split(',')[1];
                            lect.Email = lines[i].Split(',')[2];
                            lect.ContactNo = Int32.Parse(lines[i].Split(',')[3]);
                            lect.CreatedAt = DateTime.Now;
                            lect.CreatedBy = User.Identity.GetUserName();
                            lect.UpdatedAt = DateTime.Now;
                            lect.UpdatedBy = User.Identity.GetUserName();

                            //check through and validate all details
                            //check staff id
                            var match = Regex.Match(lect.StaffID, "^[s0-9]{8,8}$");
                            if (!match.Success)
                            {
                                ModelState.AddModelError("", "One of the staff id is invalid");
                                return View();
                            }

                            //check contact no.
                            match = Regex.Match(lect.ContactNo.ToString(), "^[0-9]{8,8}$");
                            if (!match.Success)
                            {
                                ModelState.AddModelError("", "One of the contact number is invalid");
                                return View();
                            }

                            lectlist.Add(lect);

                            var user = new ApplicationUser { UserName = lect.StaffID, Email = lect.Email };
                            user.EmailConfirmed = true;
                            var result = await UserManager.CreateAsync(user, "P@ssw0rd"); //default password
                            if (result.Succeeded)
                            {
                                UserManager.AddToRole(user.Id, "Lecturer");
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
                }
                catch (Exception bulkEx)
                {
                    ModelState.AddModelError("", "Error while process bulk upload. Ensure data and format is correct.");
                    return View();
                }
                db.Lecturers.AddRange(lectlist);
                db.SaveChanges();
                DeleteCSV(fileName);
            }
            else
            {
                // Upload file is invalid
                string err = "Uploaded file is invalid! Please try again!";
                TempData["InputWarning"] = err;
                return View();
            }

            return RedirectToAction("ManageLecturer");
        }

        public ActionResult UpdateLecturer(string id)
        {
            UpdateLecturerViewModel ulvm = new UpdateLecturerViewModel();
            List<AssignmentClass> ac = new List<AssignmentClass>();

            //Get Lecturer
            Lecturer lecturer = db.Lecturers.ToList().Find(l => l.StaffID == id && l.DeletedAt == null);
            List<Lec_Class> lc = db.Lec_Class.Where(lec => lec.StaffID == id).ToList();
            List<Class> allClasses = db.Classes.ToList().FindAll(c => c.DeletedAt == null);

            foreach (var c in allClasses)
            {
                AssignmentClass a = new AssignmentClass();

                String courseAbbr = db.Courses.Where(courses => courses.CourseID == c.CourseID).FirstOrDefault().CourseAbbr;
                String className = courseAbbr + "/" + c.ClassName;

                a.ClassName = className;

                a.ClassId = c.ClassID;
                if (lc.FindAll(lec => lec.ClassID == c.ClassID).Count() > 0)
                {
                    a.isSelected = true;
                }
                else
                {
                    a.isSelected = false;
                }
                ac.Add(a);
            }

            ulvm.ClassList = ac;
            ulvm.StaffID = id;
            ulvm.Name = lecturer.Name;
            ulvm.ContactNo = lecturer.ContactNo;

            return View(ulvm);
        }

        [HttpPost]
        public ActionResult UpdateLecturer(UpdateLecturerViewModel model, string command, string StaffID)
        {
            Lecturer lecturer = db.Lecturers.ToList().Find(l => l.StaffID == StaffID);
            //List<Lecturer> Lecturers = db.Lecturers.ToList();
            List<Lec_Class> lc = db.Lec_Class.Where(lec => lec.StaffID == StaffID).ToList();

            if (command.Equals("Update"))
            {
                lecturer.Name = model.Name;
                lecturer.ContactNo = model.ContactNo;
                lecturer.UpdatedAt = DateTime.Now;
                lecturer.UpdatedBy = User.Identity.Name;

                //clear data from lecturer-class relation table before adding new ones
                foreach (Lec_Class lec in lc)
                {
                    db.Lec_Class.Remove(lec);
                }

                foreach (AssignmentClass ac in model.ClassList)
                {
                    if (ac.isSelected == true)
                    {
                        db.Lec_Class.Add(new Lec_Class
                        {
                            ClassID = ac.ClassId,
                            StaffID = model.StaffID
                        });
                    }
                }

                db.SaveChanges();
            }
            else
            {
                if (db.Assignments.Where(assgn => assgn.CreateBy == StaffID).Count() == 0)
                {
                    AspNetUser user = db.AspNetUsers.Where(u => u.UserName == StaffID).FirstOrDefault();
                    db.AspNetUserRoles.Remove(db.AspNetUserRoles.Where(ur => ur.UserId == user.Id).FirstOrDefault());
                    db.AspNetUsers.Remove(user);

                    if (db.Lec_Class.Where(lcc => lcc.StaffID == StaffID).Count() > 0)
                    {
                        db.Lec_Class.Remove(db.Lec_Class.Where(lcc => lcc.StaffID == StaffID).FirstOrDefault());
                    }

                    //db.Lecturers.Remove(db.Lecturers.Where(l => l.StaffID == StaffID).FirstOrDefault());
                    lecturer.DeletedAt = DateTime.Now;
                    lecturer.DeletedBy = User.Identity.Name;

                    db.SaveChanges();
                    return RedirectToAction("ManageLecturer");
                }
                else
                {
                    ModelState.AddModelError("DeleteError", "There are still assignments or classes tied to this lecturer's account. You have to purge data from the database before deleting.");
                    return View(model);
                }
            }

            return RedirectToAction("ManageLecturer");
        }

        //Manage + Add/Bulk Add + Update/Delete Students
        public ActionResult ManageStudent()
        {
            List<ManageStudentViewModel> lvm = new List<ManageStudentViewModel>();

            List<Student> x = new List<Student>();
            x = db.Students.Where(a => a.DeletedAt == null).ToList();

            foreach (Student i in x)
            {
                ManageStudentViewModel vm = new ManageStudentViewModel();

                vm.AdminNo = i.AdminNo.ToUpper();
                vm.Name = i.Name;
                vm.ContactNo = i.ContactNo.ToString();
                vm.Email = i.Email;

                int courseId = db.Classes.ToList().Find(cs => cs.ClassID == i.ClassID).CourseID;
                string className = db.Classes.ToList().Find(cs => cs.ClassID == i.ClassID).ClassName;
                string courseAbbr = db.Courses.ToList().Find(cs => cs.CourseID == courseId).CourseAbbr;


                vm.Class = courseAbbr + "/" + className;
                vm.CreatedBy = i.CreatedBy.ToUpper();

                lvm.Add(vm);
            }

            return View(lvm);

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
            try
            {
                var user = new ApplicationUser { UserName = model.AdminNo, Email = model.Email };
                user.EmailConfirmed = true;
                var result = await UserManager.CreateAsync(user, "P@ssw0rd"); //default password
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Student");
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
            return RedirectToAction("ManageStudent", "Admin");
        }

        public ActionResult BulkAddStudent()
        {
            return View();
        }

        public FileResult DownloadBulkAddStudentFile()
        {
            string f = Server.MapPath(@"~/BulkUploadFiles/BulkAddStudent.csv");
            byte[] fileBytes = System.IO.File.ReadAllBytes(f);
            string fileName = "BulkAddStudent.csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public async Task<ActionResult> BulkAddStudent(HttpPostedFileBase file)
        {
            if ((file != null && Path.GetExtension(file.FileName) == ".csv") && (file.ContentLength > 0))
            {
                //Upload and save the file
                DirectoryInfo di;
                int subNum = 0;
                //check how many files are inside, get the amount 
                if ((di = new DirectoryInfo(Server.MapPath(@"~/TempStudentCSV/"))).Exists)
                {
                    subNum = di.GetFiles().Count();
                }
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                fileName = fileName + subNum + ".csv";
                //store the file inside TempCSV
                //we name the file as fileName + number of files + 1 to give it an unique identifier
                var path = Path.Combine(Server.MapPath(@"~/TempStudentCSV/"), fileName);
                file.SaveAs(path);

                string[] lines = System.IO.File.ReadAllLines(path);
                List<Student> slist = new List<Student>();

                try
                {
                    for (int i = 1; i < lines.Length; i++)
                    {

                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            Student s = new Student();
                            //s.ClassID = Int32.Parse(lines[i].Split(',')[0]);

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
                }
                catch (Exception BulkEx)
                {
                    ModelState.AddModelError("", "Error while process bulk upload. Ensure data and format is correct.");
                    return View();
                }

                db.Students.AddRange(slist);
                db.SaveChanges();
                DeleteCSV(fileName);
            }
            else
            {
                // Upload file is invalid
                string err = "Uploaded file is invalid! Please try again!";
                TempData["InputWarning"] = err;
                return View();
            }
            return RedirectToAction("ManageStudent");
        }

        public ActionResult UpdateStudent(string id)
        {
            ViewModels.Admin.UpdateStudentViewModel model = new ViewModels.Admin.UpdateStudentViewModel();

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
            Student student = db.Students.ToList().Find(st => st.AdminNo == id && st.DeletedAt == null);

            model.AdminNo = student.AdminNo;
            model.Name = student.Name;
            model.ClassID = student.ClassID;
            model.ContactNo = student.ContactNo;
            model.Email = student.Email;
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateStudent(ViewModels.Admin.UpdateStudentViewModel model, string command, string AdminNo)
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
                return RedirectToAction("ManageStudent");
            }
            //Delete Student
            else
            {
                if (db.Submissions.Where(sub => sub.AdminNo == AdminNo).Count() == 0)
                {
                    AspNetUser user = db.AspNetUsers.Where(u => u.UserName == AdminNo).FirstOrDefault();
                    db.AspNetUserRoles.Remove(db.AspNetUserRoles.Where(ur => ur.UserId == user.Id).FirstOrDefault());
                    db.AspNetUsers.Remove(user);

                    student.DeletedAt = DateTime.Now;
                    student.DeletedBy = User.Identity.Name;

                    db.SaveChanges();
                    return RedirectToAction("ManageStudent");
                }
                else
                {
                    ModelState.AddModelError("DeleteError", "There are still submissions that are tied to this student's account. " +
                        "You have to purge all submissions made by this student before deleting.");
                    return View(model);
                }
            }
        }

        //Manage + Add + Update/Delete Modules 
        public ActionResult ManageModule()
        {
            List<ManageModuleViewModel> lmmvm = new List<ManageModuleViewModel>();

            List<Module> m = new List<Module>();
            m = db.Modules.Where(mod => mod.DeletedAt == null).ToList();

            foreach (Module i in m)
            {
                ManageModuleViewModel mmvm = new ManageModuleViewModel();

                mmvm.ModuleCode = i.ModuleCode;
                mmvm.ModuleName = i.ModuleName;
                mmvm.ProgrammingLanguage = db.ProgLanguages.ToList().Find(p => p.LanguageId == i.LanguageId).LangageType;
                mmvm.CreatedBy = i.CreatedBy.ToUpper();

                lmmvm.Add(mmvm);
            }

            return View(lmmvm);
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
                TempData["Error"] = "Failed to save module. Please try again later with a valid module code.";
                return View(addModuleVM);
            }

            return RedirectToAction("ManageModule");
        }

        public ActionResult UpdateModule(string id)
        {
            AddModuleViewModel umvm = new AddModuleViewModel();
            Module module = db.Modules.ToList().Find(m => m.ModuleCode == id && m.DeletedAt == null);
            umvm.ModuleCode = module.ModuleCode;
            umvm.ModuleName = module.ModuleName;
            umvm.ProgLangId = module.LanguageId;
            umvm.Languages = db.ProgLanguages.ToList();
            return View(umvm);
        }

        [HttpPost]
        public ActionResult UpdateModule(AddModuleViewModel model, string command)
        {
            try
            {
                if (command.Equals("Update"))
                {
                    Module module = db.Modules.ToList().Find(m => m.ModuleCode == model.ModuleCode);
                    module.ModuleName = model.ModuleName;
                    module.UpdatedAt = DateTime.Now;
                    module.UpdatedBy = User.Identity.Name;
                    db.SaveChanges();
                }
                else if (command.Equals("Delete"))
                {
                    //check if there is any assignment that is still tied to it
                    if (db.Assignments.ToList().FindAll(a => a.ModuleCode == model.ModuleCode && a.DeletedAt == null).Count == 0)
                    {
                        Module module = db.Modules.ToList().Find(m => m.ModuleCode == model.ModuleCode);
                        module.DeletedAt = DateTime.Now;
                        module.DeletedBy = User.Identity.Name;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("DeleteError", "An assignment belonging to this module is still active, please delete that assignment before attempting to "
                            + "delete this module.");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to update module. Please try again!";
                return View(model);
            }

            //run successfully
            return RedirectToAction("ManageModule");
        }

        //Manage + Add + Update/Delete Course
        public ActionResult ManageCourse()
        {
            List<ManageCourseViewModel> lmcvm = new List<ManageCourseViewModel>();

            List<Course> c = new List<Course>();
            c = db.Courses.Where(a => a.DeletedAt == null).ToList();

            foreach (Course i in c)
            {
                ManageCourseViewModel mcvm = new ManageCourseViewModel();

                mcvm.CourseId = i.CourseID.ToString();
                mcvm.CourseName = i.CourseName;
                mcvm.Abbreviation = i.CourseAbbr;
                mcvm.CreatedBy = i.CreatedBy.ToUpper();
                mcvm.ClassCount = db.Classes.Where(cl => cl.CourseID == i.CourseID).Count().ToString();

                lmcvm.Add(mcvm);
            }

            return View(lmcvm);
        }

        public ActionResult AddCourse()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCourse(AddCourseViewModel aCVM)
        {
            Course c = new Course();

            try
            {
                c.CourseName = aCVM.CourseName;
                c.CourseAbbr = aCVM.CourseAbv;
                c.CreatedBy = User.Identity.GetUserName();
                c.CreatedAt = DateTime.Now;
                c.UpdatedBy = User.Identity.GetUserName();
                c.UpdatedAt = DateTime.Now;
                db.Courses.Add(c);

                db.SaveChanges();

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to save module. Please try again!";
                return View(aCVM);
            }
            return RedirectToAction("ManageCourse", "Admin");
        }

        public ActionResult UpdateCourse(int id)
        {
            AddCourseViewModel acvm = new AddCourseViewModel();
            Course c = db.Courses.Where(course => course.CourseID == id && course.DeletedAt == null).FirstOrDefault();
            acvm.CourseAbv = c.CourseAbbr;
            acvm.CourseName = c.CourseName;
            acvm.CourseID = id;
            return View(acvm);
        }

        [HttpPost]
        public ActionResult UpdateCourse(AddCourseViewModel model, string command, int CourseID)
        {
            Course c = db.Courses.Where(course => course.CourseID == CourseID).FirstOrDefault();

            if (db.Classes.ToList().FindAll(classs => classs.CourseID == model.CourseID).Count() == 0)
            {
                if (command.Equals("Update"))
                {
                    c.CourseAbbr = model.CourseAbv;
                    c.CourseName = model.CourseName;
                    c.UpdatedAt = DateTime.Now;
                    c.UpdatedBy = User.Identity.Name;

                    db.SaveChanges();
                    return RedirectToAction("ManageCourse");
                }
                else
                {
                    c.DeletedAt = DateTime.Now;
                    c.DeletedBy = User.Identity.Name;

                    db.SaveChanges();
                    return RedirectToAction("ManageCourse");
                }
            }
            else
            {
                ModelState.AddModelError("", "Unable to delete or update course as there are still active classes that belongs to this course.");
                return View(model);
            }
        }

        //Manage + Add + Update/Delete Admins 
        public ActionResult ManageAdmin()
        {
            List<ManageAdminViewModel> lmavm = new List<ManageAdminViewModel>();

            List<Admin> a = new List<Admin>();
            a = db.Admins.Where(ad => ad.DeletedAt == null).ToList();

            foreach (Admin i in a)
            {
                ManageAdminViewModel mavm = new ManageAdminViewModel();

                mavm.AdminID = i.AdminID.ToUpper();
                mavm.Name = i.FullName;
                mavm.ContactNo = i.ContactNo.ToString();
                mavm.Email = i.Email;
                mavm.CreatedBy = i.CreatedBy.ToUpper();

                lmavm.Add(mavm);
            }

            return View(lmavm);
        }

        public ActionResult AddOneAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOneAdmin(AddAdminViewModel model)
        {
            //create default account
            var user = new ApplicationUser { UserName = model.AdminID, Email = model.Email };
            user.EmailConfirmed = true;
            var result = await UserManager.CreateAsync(user, "P@ssw0rd"); //default password
            if (result.Succeeded)
            {
                UserManager.AddToRole(user.Id, "Admin");
                Admin admin = new Admin();
                admin.AdminID = model.AdminID;
                admin.FullName = model.FullName;
                admin.ContactNo = model.ContactNo;
                admin.Email = model.Email;
                admin.CreatedAt = DateTime.Now;
                admin.CreatedBy = User.Identity.Name;
                admin.UpdatedAt = DateTime.Now;
                admin.UpdatedBy = User.Identity.Name;

                db.Admins.Add(admin);
                db.SaveChanges();

                return RedirectToAction("ManageAdmin", "Admin");
            }
            else
            {
                //display identity framework error
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(model);
            }
        }

        public ActionResult UpdateAdmin(string id)
        {
            UpdateAdminViewModel model = new UpdateAdminViewModel();
            //Get Lecturer
            Admin admin = db.Admins.ToList().Find(ad => ad.AdminID == id && ad.DeletedAt == null);
            model.AdminID = id;
            model.FullName = admin.FullName;
            model.ContactNo = admin.ContactNo;

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateAdmin(UpdateAdminViewModel model, string command,
            string AdminID)
        {
            Admin admin = db.Admins.ToList().Find(ad => ad.AdminID == AdminID);
            if (command.Equals("Update"))
            {
                admin.FullName = model.FullName;
                admin.ContactNo = model.ContactNo;
                admin.UpdatedAt = DateTime.Now;
                admin.UpdatedBy = User.Identity.Name;

                db.SaveChanges();
            }
            else
            {
                if (admin.AdminID == User.Identity.Name)
                {
                    ModelState.AddModelError("", "You are not allowed to delete your own account.");
                    return View(model);
                }
                else
                {
                    AspNetUser user = db.AspNetUsers.Where(u => u.UserName == AdminID).FirstOrDefault();
                    db.AspNetUserRoles.Remove(db.AspNetUserRoles.Where(ur => ur.UserId == user.Id).FirstOrDefault());
                    db.AspNetUsers.Remove(user);

                    admin.DeletedAt = DateTime.Now;
                    admin.DeletedBy = User.Identity.Name;

                    db.SaveChanges();
                }
            }

            return RedirectToAction("ManageAdmin");
        }

        //Purge Files 
        public ActionResult Purge()
        {
            PurgeViewModel pvm = new PurgeViewModel();

            pvm.allAssignments = db.Assignments.ToList();
            pvm.allSubmission = db.Submissions.ToList();
            pvm.allClasses = db.Classes.ToList();
            foreach (Class c in pvm.allClasses)
            {
                String courseAbbr = db.Courses.Where(courses => courses.CourseID == c.CourseID).FirstOrDefault().CourseAbbr;
                String className = courseAbbr + "/" + c.ClassName;

                c.ClassName = className;
            }
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
        }
    }
}