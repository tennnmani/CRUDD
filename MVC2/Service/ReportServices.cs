using Microsoft.EntityFrameworkCore;
using MVC2.Interface;
using MVC2.Models;
using MVC2.ViewModels;
using System.Linq;

namespace MVC2.Service
{
    public class ReportServices : IReport
    {
        private readonly DatabaseContext _context;

        public ReportServices(DatabaseContext context)
        {
            _context = context;
        }

        public List<MasterVM> getJoinedDateCount()
        {
            var studentJoined = (from student in _context.Students
                               group student.StudentId by student.CreatedDate.Date into date
                               select new MasterVM { Name = date.Key.ToString(), Value = date.Count() }).ToList();

            var alterstdJoin = _context.Students
                                 .GroupBy(s => s.CreatedDate.Date)
                                 .Select(c => new MasterVM { Name = c.Key.ToString(), Value = c.Count() }).ToList();
            
            return studentJoined;
        }

        public List<AgeVM> gradeAge()
        {
            //var avrageAgeList = _context.Students.Include(s => s.Grade)
            //                       .GroupBy(s => s.Grade.GradeName)
            //                       .Select(i => new MasterVM { Name = i.Key, Value = Convert.ToDecimal(i.Average(i => i.Age)) }).ToList();

            //var maxAgeList = _context.Students.Include(s => s.Grade)
            //              .GroupBy(s => s.Grade.GradeName)
            //              .Select(i => new MasterVM { Name = i.Key, Value = i.Max(i => i.Age) }).ToList();

            //var minAgeList = _context.Students.Include(s => s.Grade)
            //                     .GroupBy(s => s.Grade.GradeName)
            //                     .Select(i => new MasterVM { Name = i.Key, Value = i.Min(i => i.Age) }).ToList();

            //var ageSumList = _context.Students.Include(s => s.Grade)
            //                    .GroupBy(s => s.Grade.GradeName)
            //                    .Select(c => new MasterVM { Name = c.Key, Value = c.Sum(s => s.Age) }).ToList();


            var x = (from student in _context.Students
                     join grade in _context.Grades on student.GradeId equals grade.GradeId
                     group student.Age by grade.GradeName into agestudent
                     select new AgeVM
                     {
                         Name = agestudent.Key,
                         AverageAge = Convert.ToDecimal(agestudent.Average()),
                         MinAge = agestudent.Min(),
                         MaxAge = agestudent.Max(),
                         SumAge = agestudent.Sum(),
                     }).ToList();

            return _context.Students.Include(s => s.Grade)
                                   .GroupBy(s => s.Grade.GradeName)
                                   .Select(i => new AgeVM
                                   {
                                       Name = i.Key,
                                       AverageAge = Convert.ToDecimal(i.Average(i => i.Age)),
                                       MaxAge = i.Max(i => i.Age),
                                       MinAge = i.Min(i => i.Age),
                                       SumAge = i.Sum(i => i.Age),
                                   }).ToList();
        }

        public List<MasterVM> gradeAgeAvrage()
        {
            var avrageAgeList = _context.Students.Include(s => s.Grade)
                                   .GroupBy(s => s.Grade.GradeName)
                                   .Select(i => new MasterVM { Name = i.Key, Value = Convert.ToDecimal(i.Average(i => i.Age)) }).ToList();

            var aleteavgage = (from student in _context.Students
                              join grade in _context.Grades on student.GradeId equals grade.GradeId
                              group student.Age by grade.GradeName into agestudent
                              select new MasterVM { Name = agestudent.Key, Value = Convert.ToDecimal(agestudent.Average()) }).ToList();

            return avrageAgeList;
        }

        public List<MasterVM> gradeAgeMax()
        {
            var maxAgeList = _context.Students.Include(s => s.Grade)
                              .GroupBy(s => s.Grade.GradeName)
                              .Select(i => new MasterVM { Name = i.Key, Value = i.Max(i => i.Age) }).ToList();

            var aletemaxage = (from student in _context.Students
                              join grade in _context.Grades on student.GradeId equals grade.GradeId
                              group student.Age by grade.GradeName into agestudent
                              select new MasterVM { Name = agestudent.Key, Value = agestudent.Max() }).ToList();
            return maxAgeList;
        }

        public List<MasterVM> gradeAgeMin()
        {
            var minAgeList = _context.Students.Include(s => s.Grade)
                                 .GroupBy(s => s.Grade.GradeName)
                                 .Select(i => new MasterVM { Name = i.Key, Value = i.Min(i => i.Age) }).ToList();

            var aleteminage = (from student in _context.Students
                              join grade in _context.Grades on student.GradeId equals grade.GradeId
                              group student.Age by grade.GradeName into agestudent
                              select new MasterVM { Name = agestudent.Key, Value = agestudent.Min() }).ToList();

            return minAgeList;
        }

        public List<MasterVM> gradeAgeSum()
        {
            var ageSumList = _context.Students.Include(s => s.Grade)
                                 .GroupBy(s => s.Grade.GradeName)
                                 .Select(c => new MasterVM { Name = c.Key, Value = c.Sum(s => s.Age) }).ToList();


            var aterageSum = (from student in _context.Students
                             join grade in _context.Grades on student.GradeId equals grade.GradeId
                             group student.Age by grade.GradeName into gradestudent
                             select new MasterVM { Name = gradestudent.Key, Value = gradestudent.Sum() }).ToList();

            return ageSumList;
        }

        public List<MasterVM> studentSubjectCount()
        {
            var studentSubCount = (from gradesub in _context.GradeSubjects
                                join student in _context.Students on gradesub.GradeId equals student.GradeId
                                join subject in _context.Subjects on gradesub.SubjectId equals subject.SubjectId
                                group student.StudentId by subject.SubjectName into sublearned
                                select new MasterVM { Name = sublearned.Key.ToString(), Value = sublearned.Count() }).ToList();

            return studentSubCount;
        }

    }
}
