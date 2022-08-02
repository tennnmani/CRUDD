using MVC2.Models;

namespace MVC2.Interface
{
    public interface IStudent
    {
        IQueryable<Student> getFiltteredStudent(string searchs, string fromDate, string toDate);
        Task<Student> getStudentWGradeNSub(int id);
        Task<Student> getStudent(int? id);
        Task  updateStudentAsync(Student s);
        Task  createStudent(Student s);
        Task removeStudent(Student s);
        int count();
    }
}
