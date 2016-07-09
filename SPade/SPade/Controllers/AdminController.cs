using SPade.Models.DAL;
using SPade.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPade.Controllers
{
    public class AdminController : Controller
    {

        


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

                var db = new SPadeDBEntities2();
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
            return View();
        }


        [HttpPost]
        public ActionResult AddOneClass(AddClassViewModel model)
        {
            try
            {

                var db = new SPadeDBEntities2();
                var class1 = new Class()
                {
                    ClassName = model.ClassName,
                    CourseID = 1,            
                    CreatedBy = "Admin",
                    UpdatedBy = "Admin",
                    DeletedBy = "Admin",                   
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = DateTime.Now

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
            return View();
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
            return View();
        }
        public ActionResult UpdateStudent()
        {
            return View();
        }
        public ActionResult UpdateLecturer()
        {
            return View();
        }






    }
}