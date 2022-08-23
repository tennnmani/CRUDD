using Microsoft.EntityFrameworkCore;
using MVC2.Interface;
using MVC2.Models;
using MVC2.ViewModels;
using System.Linq;

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

        public async Task<Student> getStudentWGradeNSub(int id, string name)
        {

            var students = _context.Students.FromSqlRaw($"Exec GetStudentById  @id = {id} , @Name = {name}").ToList();

            #region comments

            //count
            //var studentJoined = _context.Students
            //             .GroupBy(s => s.CreatedDate.Date)
            //             .Select(c => new MasterVM { Name = c.Key.ToString(), Value = c.Count() }).ToList();


            //var alterstdJoin = from student in _context.Students
            //                   group student.StudentId by student.CreatedDate.Date into date
            //                   select new MasterVM { Name = date.Key.ToString(), Value = date.Count() };


            //
            //var alterSubCount = from gradesub in _context.GradeSubjects
            //                    join student in _context.Students on gradesub.GradeId equals student.GradeId
            //                    join subject in _context.Subjects on gradesub.SubjectId equals subject.SubjectId
            //                    group student.StudentId by subject.SubjectName into sublearned
            //                    select new MasterVM { Name = sublearned.Key.ToString(), Value = sublearned.Count() };


            //sum
            //var ageSumList = _context.Students.Include(s => s.Grade)
            //            .GroupBy(s => s.Grade.GradeName)
            //            .Select(c => new MasterVM { Name = c.Key, Value = c.Sum(s => s.Age) });


            //var aterageSum = from student in _context.Students
            //                 join grade in _context.Grades on student.GradeId equals grade.GradeId
            //                 group student.Age by grade.GradeName into gradestudent
            //                 select new MasterVM { Name = gradestudent.Key, Value = gradestudent.Sum() };


            // min max

            //var minAgeList = _context.Students.Include(s => s.Grade)
            //                .GroupBy(s => s.Grade.GradeName)
            //                .Select(i => new { grade = i.Key, min = i.Min(i => i.Age) });

            //var aleteminage = from student in _context.Students
            //                  join grade in _context.Grades on student.StudentId equals grade.GradeId
            //                  group student.Age by grade.GradeName into agestudent
            //                  select new MasterVM { Name = agestudent.Key, Value = agestudent.Min() };


            //var maxAgeList = _context.Students.Include(s => s.Grade)
            //          .GroupBy(s => s.Grade.GradeName)
            //          .Select(i => new { grade = i.Key, max = i.Max(i => i.Age) });

            //var aletemaxage = from student in _context.Students
            //                  join grade in _context.Grades on student.StudentId equals grade.GradeId
            //                  group student.Age by grade.GradeName into agestudent
            //                  select new MasterVM { Name = agestudent.Key, Value = agestudent.Max() };



            //var avrageAgeList = _context.Students.Include(s => s.Grade)
            //          .GroupBy(s => s.Grade.GradeName)
            //          .Select(i => new { grade = i.Key, avg = i.Average(i => i.Age) });

            //var aleteavgage = from student in _context.Students
            //                  join grade in _context.Grades on student.StudentId equals grade.GradeId
            //                  group student.Age by grade.GradeName into agestudent
            //                  select new MasterVM { Name = agestudent.Key, Value = Convert.ToDecimal(agestudent.Average()) };

            #endregion

            // join with multiple condition
            var result = (from subject in _context.Subjects
                          join gradeSubject in _context.GradeSubjects on subject.SubjectId equals gradeSubject.SubjectId into gradesub
                          from gradeSubject in gradesub.DefaultIfEmpty() // left join
                          join grade in _context.Grades on new { key1 = gradeSubject.GradeId, key2 = subject.CreatedDate }
                             equals new { key1 = grade.GradeId, key2 = grade.CreatedDate }
                          select new
                          {
                              subject.SubjectName,
                              grade.GradeName,
                              SubjectCreated = subject.CreatedDate,
                              GradeCreated = grade.CreatedDate
                          }).ToList();

            return await _context.Students.Include(g => g.Grade)
                                    .ThenInclude(st => st.SubjectsTaught)
                                        .ThenInclude(i => i.Subject)
                                    .SingleAsync(s => s.StudentId == id);



            //left join and multiple coloum join
            //group by / sum / max min
            // daily count  

            // parameterized stored procidured -- get
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
