﻿using SPade.Models;
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
        public ActionResult UpdateStudent()
        {
            UpdateStudentViewModel model = new UpdateStudentViewModel();
            string x = "p3333333";
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





    }
}