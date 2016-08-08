using System.ComponentModel.DataAnnotations;

namespace SPade.ViewModels.Admin
{
    public class AddCourseViewModel
    {
        public int CourseID { get; set; }
        [Required(ErrorMessage = "Please do not leave Course Name blank !")]
        [StringLength(200)]
        public string CourseName { get; set; }
        [Required(ErrorMessage = "Please do not leave Course Abbreviation blank !")]
        [StringLength(10)]
        public string CourseAbv { get; set; }

    }
}