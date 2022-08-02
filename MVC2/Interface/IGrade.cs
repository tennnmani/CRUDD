using MVC2.Models;

namespace MVC2.Interface
{
    public interface IGrade
    {
        IQueryable<Grade> getFiltteredGrade(string searchs, string fromDate, string toDate);
        Task<Grade> getGrade(int? id);
        Task<Grade> getGradeWSubject(int? id);
        Task updateGradeAsync(Grade g, string[] subjectIndex);
        Task createGrade(Grade g, string[] subjectIndex);
        Task removeGrade(Grade g);
        int count();
        IQueryable<Grade> getAllGrade();
    }
}
