using Microsoft.EntityFrameworkCore;
using MVC2.Interface;
using MVC2.Models;

namespace MVC2.Service
{
    public class GradeServices : IGrade
    {
        private readonly DatabaseContext _context;

        public GradeServices(DatabaseContext context)
        {
            _context = context;
        }

        public int count()
        {
            return _context.Grades.Count();
        }

        public async Task createGrade(Grade grade, string[] subjectIndex)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(grade);
                    await _context.SaveChangesAsync();

                    foreach (var i in subjectIndex)
                    {
                        var gradeSubject = new GradeSubject();
                        gradeSubject.SubjectId = Int32.Parse(i);
                        gradeSubject.GradeId = grade.GradeId;
                        _context.Add(gradeSubject);
                        await _context.SaveChangesAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public IQueryable<Grade> getAllGrade()
        {
            return _context.Grades.AsNoTracking();
        }

        public IQueryable<Grade> getFiltteredGrade(string searchs, string fromDate, string toDate)
        {
            var grade = _context.Grades.Include(g => g.SubjectsTaught)
                            .ThenInclude(s => s.Subject).AsNoTracking();

            if (!String.IsNullOrEmpty(searchs))
            {
                grade = grade.Where(s => s.GradeName.Contains(searchs));
            }

            if (!String.IsNullOrEmpty(fromDate) && !String.IsNullOrEmpty(toDate))
            {
                grade = grade.Where(s => s.CreatedDate.Date >= DateTime.Parse(fromDate) && s.CreatedDate.Date <= DateTime.Parse(toDate));
            }

            return grade;
        }

        public async Task<Grade> getGrade(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return await _context.Grades.FindAsync(id);
        }

        public async Task<Grade> getGradeWSubject(int? id)
        {
            return await _context.Grades.Include(s => s.SubjectsTaught).FirstAsync(g => g.GradeId == id);
        }

        public async Task removeGrade(Grade grade)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var gradeSubject = _context.GradeSubjects.Where(g => g.GradeId == g.GradeId);
                    _context.RemoveRange(gradeSubject);

                    _context.Grades.Remove(grade);

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {

                }

            }
        }

        public async Task updateGradeAsync(Grade grade, string[] subjectIndex)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Update(grade);

                    //delete grade subject
                    var gS = _context.GradeSubjects.Where(g => g.GradeId == grade.GradeId);
                    _context.RemoveRange(gS);

                    foreach (var i in subjectIndex)
                    {
                        var sG = new GradeSubject();
                        sG.SubjectId = Int32.Parse(i);
                        sG.GradeId = grade.GradeId;
                        _context.Add(sG);
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                }
                catch (DbUpdateException ex)
                {

                }
            }
        }
    }
}
