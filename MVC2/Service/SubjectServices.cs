using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MVC2.Interface;
using MVC2.Models;

namespace MVC2.Service
{
    public class SubjectServices : ISubject
    {
        private readonly DatabaseContext _context;

        public SubjectServices(DatabaseContext context)
        {
            _context = context;
        }

        public int count()
        {
            return _context.Subjects.Count();
        }

        public async Task createSubject(Subject s)
        {
            try
            {
                _context.Add(s);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        public  IQueryable<Subject> getAllSubject()
        {
            return _context.Subjects.AsNoTracking();
        }

        public IQueryable<Subject> getFiltteredSubject(string searchs, string fromDate, string toDate)
        {
            var subjects = _context.Subjects
                          .Include(s => s.GradeSubject)
                              .ThenInclude(i => i.Grade).AsNoTracking();

            if (!String.IsNullOrEmpty(searchs))
            {
                subjects = subjects.Where(s => s.SubjectName.Contains(searchs));
            }

            if (!String.IsNullOrEmpty(fromDate) && !String.IsNullOrEmpty(toDate))
            {
                subjects = subjects.Where(s => s.CreatedDate.Date >= DateTime.Parse(fromDate) && s.CreatedDate.Date <= DateTime.Parse(toDate));
            }

            return subjects;
        }

        public async Task<Subject> getSubject(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return await _context.Subjects.FindAsync(id);
        }

        public async Task removeSubject(Subject s)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var gS = _context.GradeSubjects.Where(g => g.SubjectId == s.SubjectId);
                    _context.GradeSubjects.RemoveRange(gS);

                    _context.Subjects.Remove(s);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)

                }
            }
        }

        public async Task updateSubject(Subject s)
        {
            try
            {
                _context.Update(s);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {

            }
        }

    }
}
