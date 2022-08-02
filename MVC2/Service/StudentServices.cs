using Microsoft.EntityFrameworkCore;
using MVC2.Interface;
using MVC2.Models;

namespace MVC2.Service
{
    public class StudentServices : IStudent
    {
        private readonly DatabaseContext _context;

        public StudentServices(DatabaseContext context)
        {
            _context = context;
        }

        public int count()
        {
            return _context.Students.Count();
        }

        public IQueryable<Student> getFiltteredStudent(string searchs, string fromDate, string toDate)
        {
            var students = _context.Students.Include(g => g.Grade).AsNoTracking();

            if (!String.IsNullOrEmpty(searchs))
            {
                students = students.Where(s => s.LastName.Contains(searchs)
                                       || s.FirstName.Contains(searchs)
                                       || s.Grade.GradeName.Contains(searchs)
                                        );
            }

            if (!String.IsNullOrEmpty(fromDate) && !String.IsNullOrEmpty(toDate))
            {
                students = students.Where(s => s.CreatedDate.Date >= DateTime.Parse(fromDate) && s.CreatedDate.Date <= DateTime.Parse(toDate));
            }

            return students;
        }

        public async Task<Student> getStudent(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await _context.Students.FindAsync(id);
        }

        public async Task<Student> getStudentWGradeNSub(int id)
        {
            return await _context.Students.Include(g => g.Grade)
                                    .ThenInclude(st => st.SubjectsTaught)
                                        .ThenInclude(i => i.Subject)
                                    .AsNoTracking().SingleAsync(s => s.StudentId == id);
        }

        public async Task updateStudentAsync(Student s)
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
        public async Task removeStudent(Student s)
        {
            try
            {
                _context.Students.Remove(s);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
               
            }
        }

        public async Task createStudent(Student s)
        {
            try
            {
                _context.Add(s);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
