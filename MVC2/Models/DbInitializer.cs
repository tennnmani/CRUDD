namespace MVC2.Models
{
    public class DbInitializer
    {
        public static void Initialize(DatabaseContext context)
        {
            if (context.Students.Any())
            {
                return;
            }

            var grad = new Grade[]
            {
                new Grade{GradeName = "One"},
                new Grade{GradeName = "Two"},
                new Grade{GradeName = "Three"},
            };
            foreach (Grade g in grad)
            {
                context.Grades.Add(g);
            }
            context.SaveChanges();

            var std = new Student[]
            {
                new Student
                {
                    FirstName="Travis",
                    LastName="Barker",
                    DOB=DateTime.Parse("1990/02/03"),
                    GradeId = grad.Single(i=>i.GradeName=="One").GradeId
                },
                new Student
                {
                    FirstName="Mark",
                    LastName="Delong",
                    DOB=DateTime.Parse("1990/04/08"),
                    GradeId = grad.Single(i=>i.GradeName=="One").GradeId
                },
                new Student
                {
                    FirstName="Tom",
                    LastName="Hoppus",
                    DOB=DateTime.Parse("1992/05/01"),
                    GradeId = grad.Single(i=>i.GradeName=="Two").GradeId
                },
                new Student
                {
                    FirstName="Sky",
                    LastName="Barker",
                    DOB=DateTime.Parse("1989/02/09"),
                    GradeId = grad.Single(i=>i.GradeName=="Two").GradeId
                },
                new Student
                {
                    FirstName="Andy",
                    LastName="Lang",
                    DOB=DateTime.Parse("1996/01/03"),
                    GradeId = grad.Single(i=>i.GradeName=="Three").GradeId
                },
            };
            foreach (Student s in std)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();

            var sub = new Subject[]
            {
                new Subject{ SubjectName="Science"},
                new Subject{ SubjectName="Maths"},
                new Subject{ SubjectName="Nepali"},
                new Subject{ SubjectName="English"},
                new Subject{ SubjectName="OPT"},
            };
            foreach (Subject su in sub)
            {
                context.Subjects.Add(su);
            }
            context.SaveChanges();

            var gsub = new GradeSubject[]
            {
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="Three").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="Maths").SubjectId
                },
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="Three").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="Science").SubjectId
                },
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="Three").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="English").SubjectId
                },
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="Three").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="OPT").SubjectId
                },
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="Two").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="Maths").SubjectId
                },
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="Two").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="Science").SubjectId
                },
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="Two").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="English").SubjectId
                },
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="One").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="Nepali").SubjectId
                },
                new GradeSubject
                {
                    GradeId = grad.Single(i=>i.GradeName =="One").GradeId, 
                    SubjectId = sub.Single(i=>i.SubjectName=="English").SubjectId
                },
            };
            foreach (GradeSubject gs in gsub)
            {
                context.GradeSubjects.Add(gs);
            }
            context.SaveChanges();
        }
    }
}
