using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SPade.Models;
using SPade.Models.DAL;
using SPade.ViewModels.Admin;
using SPade.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPade.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        //  { UserID = Request.QueryString["UserID"]
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

        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }
        [HttpGet]
        public ActionResult BulkAddLecturer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BulkAddLecturer(HttpPostedFileBase file)
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
                List<Lecturer> lectlist = new List<Lecturer>();
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

                        lectlist.Add(lect);
                    }
                }
                db.Lecturers.AddRange(lectlist);
                db.SaveChanges();
            }
            else
            {
                // Upload file is invalid
                string err = "Uploaded file is invalid ! Please try again!";
                TempData["InputWarning"] = err;
                return View();
            }

            return View("ManageLecturer");
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
            }
            else
            {
                // Upload file is invalid
                string err = "Uploaded file is invalid ! Please try again!";
                TempData["InputWarning"] = err;
                return View();
            }
            return View("ManageStudent");
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
            return RedirectToAction("Dashboard");
        }

        public ActionResult AddOneStudent()
        {
            AddStudentViewModel model = new AddStudentViewModel();
            List<Class> allClasses = db.Classes.ToList().FindAll(c => c.DeletedAt == null);
            model.Classes = allClasses;
            return View(model);
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
                c.CourseAbbr = aCVM.CourseName;
                c.CreatedBy = User.Identity.GetUserName();
                c.CreatedAt = DateTime.Now;
                c.UpdatedBy = User.Identity.GetUserName();
                c.UpdatedAt = DateTime.Now;
                db.Courses.Add(c);

                db.SaveChanges();

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to save module. Please try again !";
                return View(aCVM);
            }
            return RedirectToAction("ManageCourse", "Admin");
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
        public async Task<ActionResult> AddOneLecturer(AddLecturerViewModel model, FormCollection formCollection)
        {
            try
            {
                var user = new ApplicationUser { UserName = model.StaffID, Email = model.Email };
                user.EmailConfirmed = true;
                var result = await UserManager.CreateAsync(user, "P@ssw0rd"); //default password
                if (result.Succeeded)
                {
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

                    db.Lec_Class.Add(new Lec_Class
                    {
                        ClassID = int.Parse(formCollection["ClassID"].ToString()),
                        StaffID = model.StaffID
                    });

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
            return RedirectToAction("Dashboard");
        }

        public ActionResult AddOneLecturer()
        {
            AddLecturerViewModel model = new AddLecturerViewModel();
            List<Class> allClasses = db.Classes.ToList().FindAll(c => c.DeletedAt == null);
            model.Classes = allClasses;
            return View(model);
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

            return RedirectToAction("ManageModule");
        }

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

        public ActionResult UpdateModule(string id)
        {
            if(Session["DeleteError"] != null)
            {
                ModelState.AddModelError("Delete Error", Session["DeleteError"].ToString());
                Session.RemoveAll();
            }
            AddModuleViewModel umvm = new AddModuleViewModel();
            Module module = db.Modules.ToList().Find(m => m.ModuleCode == id && m.DeletedAt == null);
            umvm.ModuleCode = module.ModuleCode;
            umvm.ModuleName = module.ModuleName;
            umvm.ProgLangId = module.LanguageId;
            umvm.Languages = db.ProgLanguages.ToList();
            return View(umvm);
        }

        [HttpPost]
        public ActionResult UpdateModule(AddModuleViewModel model)
        {
            Module module = db.Modules.ToList().Find(m => m.ModuleCode == model.ModuleCode);
            module.ModuleName = model.ModuleName;
            module.UpdatedAt = DateTime.Now;
            module.UpdatedBy = User.Identity.Name;
            db.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public ActionResult DeleteModule(string id)
        {
            //check if there is any assignment that is still tied to it
            if (db.Assignments.ToList().FindAll(a => a.ModuleCode == id && a.DeletedAt == null).Count == 0)
            {
                Module module = db.Modules.ToList().Find(m => m.ModuleCode == id);
                module.DeletedAt = DateTime.Now;
                module.DeletedBy = User.Identity.Name;
                db.SaveChanges();

                return RedirectToAction("ManageModule");
            }
            else
            {
                Session["DeleteError"] = "An assignment belonging to this module is still active, please delete that assignment before attempting to "
                    + "delete this module.";
                //return RedirectToAction("UpdateModule", "Admin", id);
                return Redirect("/admin/updatemodule/" + id);
            }
        }
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

        public ActionResult UpdateClass(string ClassID)
        {
            UpdateClassViewModel model = new UpdateClassViewModel();
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
                if (C.ClassID.Equals(ClassID))
                {
                    model.CourseID = C.CourseID;
                    model.ClassID = C.ClassID;
                    model.ClassName = C.ClassName;
                }

            }
            foreach (Lec_Class LC in all_Lec_Class)
            {
                if (LC.ClassID.Equals(ClassID))
                {
                    model.StaffID = LC.StaffID;
                }

            }


            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateClass(UpdateClassViewModel model, string command, string ClassID)
        {

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
                    if (C.ClassID.Equals(ClassID))
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
                    if (LC.ClassID.Equals(ClassID))
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
                    if (C.ClassID.Equals(ClassID))
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


        public ActionResult UpdateStudent(string AdminNo)
        {
            UpdateStudentViewModel model = new UpdateStudentViewModel();

            //Get all classes
            List<Class> allClasses = db.Classes.ToList();
            model.Classes = allClasses;

            //Get Student           
            List<Student> Students = db.Students.ToList();

            foreach (Student S in Students)
            {
                if (S.AdminNo == AdminNo)
                {
                    model.AdminNo = S.AdminNo;
                    model.Name = S.Name;
                    model.ClassID = S.ClassID;
                    model.ContactNo = S.ContactNo;
                    model.Email = S.Email;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateStudent(UpdateStudentViewModel model, string command, string AdminNo)
        {
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
                    if (S.AdminNo == AdminNo)
                    {
                        S.UpdatedBy = User.Identity.Name;
                        S.UpdatedAt = DateTime.Now;

                        if (TryUpdateModel(S, "",
                           new string[] { "Name", "ClassID", "Email", "ContactNo", "UpdatedAt", "UpdatedBy" }))
                        {
                            try
                            {
                                db.SaveChanges();
                                TempData["msg"] = "<script>alert('Updated successfully');</script>";
                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                                TempData["msg"] = "<script>alert('Unable to update successfully');</script>";
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
                    if (S.AdminNo == AdminNo)
                    {
                        S.DeletedBy = User.Identity.Name;
                        S.DeletedAt = DateTime.Now;

                        if (TryUpdateModel(S, "",
                           new string[] { "DeletedBy", "DeletedAt" }))
                        {
                            try
                            {
                                db.SaveChanges();
                                TempData["msg"] = "<script>alert('Deleted successfully');</script>";
                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                                TempData["msg"] = "<script>alert('Unable to delete successfully');</script>";
                            }
                        }
                    }

                }
            }
            return View(model);
        }

        public ActionResult UpdateLecturer(string StaffID)
        {
            UpdateLecturerViewModel model = new UpdateLecturerViewModel();
            //Get Lecturer

            List<Lecturer> Lecturers = db.Lecturers.ToList();

            foreach (Lecturer L in Lecturers)
            {
                if (L.StaffID == StaffID)
                {
                    model.StaffID = L.StaffID;
                    model.Name = L.Name;
                    model.ContactNo = L.ContactNo;
                    model.Email = L.Email;
                }
            }
            return View(model);
        }

        public FileResult DownloadBulkAddStudentFile()
        {
            string f = Server.MapPath(@"~/BulkUploadFiles/BulkAddStudent.csv");
            byte[] fileBytes = System.IO.File.ReadAllBytes(f);
            string fileName = "BulkAddStudent.csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public FileResult DownloadBulkAddLecturerFile()
        {
            string f = Server.MapPath(@"~/BulkUploadFiles/BulkAddLecturer.csv");
            byte[] fileBytes = System.IO.File.ReadAllBytes(f);
            string fileName = "BulkAddLecturer.csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public ActionResult UpdateLecturer(UpdateLecturerViewModel model, string command, string StaffID)
        {

            List<Lecturer> Lecturers = db.Lecturers.ToList();
            if (command.Equals("Update"))
            {
                //Update functionality
                foreach (Lecturer L in Lecturers)
                {
                    if (L.StaffID == StaffID)
                    {
                        //Update Lecturer
                        L.UpdatedBy = "ADMIN";
                        L.UpdatedAt = DateTime.Now;

                        if (TryUpdateModel(L, "",
                           new string[] { "Name", "Email", "ContactNo", "UpdatedBy", "UpdatedAt" }))
                        {
                            try
                            {
                                db.SaveChanges();
                                TempData["msg"] = "<script>alert('Updated successfully');</script>";

                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                                TempData["msg"] = "<script>alert('Unable to update successfully');</script>";

                            }
                        }

                    }

                }
            }
            else
            {
                //Delete functionality
                foreach (Lecturer L in Lecturers)
                {
                    if (L.StaffID == StaffID)
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
                                TempData["msg"] = "<script>alert('Deleted successfully');</script>";

                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                                TempData["msg"] = "<script>alert('Unable to delete successfully');</script>";
                            }
                        }

                    }

                }
            }

            return View(model);

        }

        public ActionResult UpdateAdmin(string AdminID)
        {
            UpdateAdminViewModel model = new UpdateAdminViewModel();
            //Get Lecturer
            List<Admin> Admins = db.Admins.ToList();

            foreach (Admin A in Admins)
            {
                if (A.AdminID.ToUpper() == AdminID)
                {
                    model.AdminID = A.AdminID;
                    model.FullName = A.FullName;
                    model.ContactNo = A.ContactNo;
                    model.Email = A.Email;

                }

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateAdmin(UpdateAdminViewModel model, string command,
            string AdminID)
        {
            List<Admin> Admins = db.Admins.ToList();
            if (command.Equals("Update"))
            {
                //Update Functionality
                foreach (Admin A in Admins)
                {
                    if (A.AdminID.ToUpper() == AdminID)
                    {

                        A.UpdatedBy = "ADMIN";
                        A.UpdatedAt = DateTime.Now;

                        //Update Lecturer
                        if (TryUpdateModel(A, "",
                           new string[] { "FullName", "Email", "ContactNo", "UpdatedBy", "UpdatedAt" }))
                        {
                            try
                            {
                                db.SaveChanges();
                                TempData["msg"] = "<script>alert('Updated successfully');</script>";
                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                                TempData["msg"] = "<script>alert('Unable to update successfully');</script>";
                            }
                        }

                    }

                }
            }
            else
            {   // Delete Function
                foreach (Admin A in Admins)
                {
                    if (A.AdminID.ToUpper() == AdminID)
                    {
                        //Update Lecturer
                        A.DeletedBy = "ADMIN";
                        A.DeletedAt = DateTime.Now;

                        if (TryUpdateModel(A, "",
                           new string[] { "DeletedBy", "DeletedAt" }))
                        {
                            try
                            {
                                db.SaveChanges();
                                TempData["msg"] = "<script>alert('Delete successfully');</script>";
                            }
                            catch (DataException /* dex */)
                            {
                                //Log the error (uncomment dex variable name and add a line here to write a log.
                                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                                TempData["msg"] = "<script>alert('Unable to delete successfully');</script>";
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

                return RedirectToAction("Dashboard");
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
        }//
    }
}