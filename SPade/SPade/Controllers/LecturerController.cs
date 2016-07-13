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
using System.Data.SqlClient;

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
            ViewResultsViewModel vrvm = new ViewResultsViewModel();

            string loggedInLecturer = "s1431489"; //temp 


            List<Class> managedClasses = db.Classes.Where(c => c.Lec_Class.Where(lc => lc.ClassID == c.ClassID).FirstOrDefault().StaffID == loggedInLecturer).ToList();

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

            var assignments = db.Database.SqlQuery<DBass>("select ca.*, a.AssgnTitle from Class_Assgn ca inner join(select * from Assignment) a on ca.AssignmentID = a.AssignmentID where classid = @inClass",
    new SqlParameter("@inClass", Class)).ToList();

            return Json(assignments);
        }


        [HttpPost]
        public ActionResult ViewResults(string Class, string Assignment)
        {


            var results = db.Database.SqlQuery<DBres>("select s1.submissionid, s1.adminno, stud.name, s1.assignmentid, s1.grade, s1.filepath from submission s1 inner join ( select adminno, max(submissionid) submissionid from submission group by adminno) s2 on s1.submissionid = s2.submissionid inner join ( select * from student where classid = @inClass) stud on s1.adminno = stud.adminno where s1.assignmentid = @inAssignment",
    new SqlParameter("@inClass", Class),
    new SqlParameter("@inAssignment", Assignment)).ToList();

            return Json(results);
        }


        private class DBass
        {
            public int ClassID { get; set; }
            public int AssignmentID { get; set; }
            public int _id { get; set; }
            public string AssgnTitle { get; set; }
        }

        private class DBres
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