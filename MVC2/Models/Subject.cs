namespace MVC2.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public DateTime CreatedDate { get; set; }


        public ICollection<GradeSubject> GradeSubject { get; set; }
    }
}
