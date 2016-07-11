using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPade.Models.DAL;
using SPade.ViewModels.Admin;
using SPade.ViewModels.Lecturer;
using SPade.ViewModels.Student;
using System.IO;

namespace SPade.Controllers
{
    public class LecturerController : Controller
    {
        //init the db
        private SPadeEntities db = new SPadeEntities();

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

            string x = "s1431489"; //temp 

            //get the classes managed by the lecturer 
            List<Class> managedClasses = db.Classes.Where(c => c.Lec_Class.Where(lc => lc.ClassID == c.ClassID).FirstOrDefault().StaffID == x).ToList();

            //get the students in that classs
            foreach (Class c in managedClasses)
            {
                //match the class ID of student wit hthe class ID of the managed Classes
                var count = db.Students.Where(s => s.ClassID == c.ClassID).Count();

                e.ClassName = c.ClassName;
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
        public ActionResult ViewStudentsByClass()
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

        //   [Authorize(Roles = "")]
        public ActionResult AddAssignment()
        {

            List<AssignmentClass> ac = new List<AssignmentClass>();
            AddAssignmentViewModel aaVM = new AddAssignmentViewModel();

            string x = "1431489"; //temp 

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
        public ActionResult AddAssignment(AddAssignmentViewModel addAssgn, IEnumerable<HttpPostedFileBase> fileList)
        {
            //insert data into db 

            foreach (var file in fileList)
            {
                if (file != null && file.ContentLength > 0)
                {
                    FileInfo fileInfo;
                    string fp = file.FileName;
                    string ext = Path.GetExtension(fp);

                    if (ext == ".xml") //test case 
                    {
                        //this is for the testcase 

                        //i put inside the testcase -> this is temporary. will be renamed after inserted into the DB
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Server.MapPath(@"~/App_Data/TestCase/" + addAssgn.AssgnTitle + "_TestCase.xml");
                        fileInfo = new FileInfo(filePath);
                        fileInfo.Directory.Create();
                        file.SaveAs(filePath);

                    }
                    else
                    {
                        //for the solution file  

                        var fileName = Path.GetFileName(file.FileName);
                        //extension at the back is dynamic. cater for other language also 
                        var filePath = Server.MapPath(@"~/App_Data/Submissions/" + addAssgn.AssgnTitle + "_Solution" + ext);
                        fileInfo = new FileInfo(filePath);
                        fileInfo.Directory.Create();
                        file.SaveAs(filePath);
                    }
                    //file.SaveAs(Path.Combine(Server.MapPath("/App_Data/Temp"), Guid.NewGuid() + Path.GetExtension(file.FileName)));
                }
            }
            return View();
        }

        //  [Authorize(Roles = "")]
        public ActionResult UpdateAssignment()
        {
            return View();
        }

        //    [Authorize(Roles = "")]
        public ActionResult ViewResults()
        {
            List<ViewResultsViewModel> viewResultsView = new List<ViewResultsViewModel>();
            
            string loggedInLecturer = "s1431489"; //temp 

            int inAssignment = 1;
            int inClass = 1;

            //get the classes managed by the lecturer 
            //List<Class> managedClasses = db.Classes.Where(c => c.Lec_Class.Where(lc => lc.ClassID == c.ClassID).FirstOrDefault().StaffID == x).ToList();

            //assignment

            List<Submission> submissions = db.Submissions.ToList();

            //get the students in that classs
            foreach (Submission s in submissions)
            {
                if (s.AssignmentID==inAssignment)
                {
                    //not done yet
                    var temp = db.Students.Where(u => u.AdminNo == s.AdminNo).Select(u => new {u.Name, u.ClassID}).FirstOrDefault();

                    if (temp.ClassID == inClass)
                    {

                        ViewResultsViewModel v = new ViewResultsViewModel();

                        v.Id = s.AdminNo.ToString().ToUpper();
                        v.Name = temp.Name;
                        v.Result = (double)s.Grade * 100 + "%";
                        v.Solution = "test";

                        viewResultsView.Add(v);
                    }
                }

            }

            return View(viewResultsView);
        }

    }//end of controller
}