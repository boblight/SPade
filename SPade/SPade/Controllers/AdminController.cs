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

namespace SPade.Controllers
{
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


        [HttpPost]
        public ActionResult AddOneStudent(AddStudentViewModel model)
        {
            try
            {
                var student = new Student()
                {
                    AdminNo = model.AdminNo,
                    Name = model.Name,
                    Email = model.Email,
                    ContactNo = model.ContactNo,
                    ClassID = '1',
                    CreatedBy = "Admin",
                    UpdatedBy = "Admin",
                    DeletedBy = "Admin",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = DateTime.Now

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
            try
            {
                var class1 = new Class()
                {
                    ClassName = model.ClassName,
                    CourseID = 1,
                    CreatedBy = "Admin",
                    UpdatedBy = "Admin",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,

                };

                db.Classes.Add(class1);
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
                    DeletedBy = "Admin",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = DateTime.Now

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
            return View();
        }
        public ActionResult ManageStudent()
        {
            return View();
        }
        public ActionResult ManageLecturer()
        {
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
            return View(model);
        }


        public ActionResult UpdateStudent()
        {
            UpdateStudentViewModel model = new UpdateStudentViewModel();
            string x = "p1234567";
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
        public ActionResult UpdateStudent(UpdateStudentViewModel model)
        {
            string x = "p1234567";

            //Get all classes
            List<Class> allClasses = db.Classes.ToList();
            model.Classes = allClasses;

            //Get Student           
            List<Student> Students = db.Students.ToList();

            foreach (Student S in Students)
            {
                if (S.AdminNo == x)
                {
                    if (TryUpdateModel(S, "",
                       new string[] { "Name", "ClassID", "Email", "ContactNo" }))
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
            return View(model);
        }
        public ActionResult UpdateLecturer()
        {
            UpdateLecturerViewModel model = new UpdateLecturerViewModel();
            //Get Lecturer

            string x = "s1431489";
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
        public ActionResult UpdateLecturer(UpdateLecturerViewModel model)
        {
            string x = "s1431489";
            List<Lecturer> Lecturers = db.Lecturers.ToList();

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

            return View(model);

        }





    }
}