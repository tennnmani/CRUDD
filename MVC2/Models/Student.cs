using System.ComponentModel.DataAnnotations;

namespace MVC2.Models
{
    public class Student
    {
        public int StudentId { get;set; }
        public string FirstName { get;set; }
        public string LastName { get;set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get;set; }

        public int GradeId { get; set; }
        public Grade Grade { get; set; }
    }
}
