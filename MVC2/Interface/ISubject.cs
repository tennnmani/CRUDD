using MVC2.Models;

namespace MVC2.Interface
{
    public interface ISubject
    {
        IQueryable<Subject> getFiltteredSubject(string searchs, string fromDate, string toDate);

        int count();
        //Task<Student> getStudentWGradeNSub(int id);
        Task<Subject> getSubject(int? id);
        Task updateSubject(Subject s);
        Task createSubject(Subject s);
        Task removeSubject(Subject s);
        IQueryable<Subject> getAllSubject();
    }
}
