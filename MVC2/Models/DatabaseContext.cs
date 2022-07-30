using Microsoft.EntityFrameworkCore;

namespace MVC2.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<GradeSubject> GradeSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student").HasKey(t => t.StudentId);
            modelBuilder.Entity<Student>().Property(p => p.FirstName).IsRequired().HasMaxLength(225);
            modelBuilder.Entity<Student>().Property(p => p.LastName).IsRequired().HasMaxLength(225);
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Grade)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GradeId);

            modelBuilder.Entity<Grade>().ToTable("Grade").HasKey(g => g.GradeId);
            modelBuilder.Entity<Grade>().Property(p => p.GradeName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Subject>().ToTable("Subject").HasKey(g => g.SubjectId);
            modelBuilder.Entity<Subject>().Property(p => p.SubjectName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<GradeSubject>()
               .HasKey(c => new { c.GradeId, c.SubjectId });
        }
    }
}
