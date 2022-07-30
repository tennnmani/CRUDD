using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC2.Helper;
using MVC2.Models;

namespace MVC2.Controllers
{
    public class StudentController : Controller
    {
        private readonly DatabaseContext _context;

        public StudentController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {

            ViewData["nameS"] = sortOrder == "Name" ? "NameD" : "Name";
            ViewData["dobS"] = sortOrder == "DOB" ? "DOBD" : "DOB";
            ViewData["gradeS"] = sortOrder == "Grade" ? "GradeD" : "Grade";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var students = (from s in _context.Students.Include(g=>g.Grade)
                            //where
                            //s.FirstName.Contains(searchString) || s.LastName.Contains(searchString) || s.Grade.GradeName.Contains(searchString)
                            select  s);

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString)
                                        || s.Grade.GradeName.Contains(searchString)
                                        );
            }


            switch (sortOrder)
            {
                case "Name":
                    students = students.OrderBy(s => s.FirstName);
                    break;
                case "NameD":
                    students = students.OrderByDescending(s => s.FirstName);
                    break;
                case "DOB":
                    students = students.OrderBy(s => s.DOB);
                    break;
                case "DOBD":
                    students = students.OrderByDescending(s => s.DOB);
                    break;
                case "Grade":
                    students = students.OrderBy(s => s.GradeId);
                    break;
                case "GradeD":
                    students = students.OrderByDescending(s => s.GradeId);
                    break;
                default:
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students.Include(g => g.Grade)
                                    .ThenInclude(st => st.SubjectsTaught)
                                        .ThenInclude(i=>i.Subject)
                                    .AsNoTracking().SingleAsync(s=>s.StudentId == id);
            //var student = (from s in _context.Students.Include(g => g.Grade)
            //                .ThenInclude(st=>st.SubjectsTaught)
            //                where s.StudentId == id
            //               select s);
            return View(student);
        }
        public IActionResult Create()
        {
            ViewData["GradeList"] = new SelectList(_context.Grades, "GradeId", "GradeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName", "LastName", "DOB", "GradeId")] Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                     _context.Add(student);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {

                }
                
            }
            ViewData["GradeList"] = new SelectList(_context.Grades, "GradeId", "GradeName");
            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if(student == null)
            {
                return NotFound();
            }

            ViewData["GradeList"] = new SelectList(_context.Grades, "GradeId", "GradeName", student.GradeId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("StudentId", "FirstName", "LastName", "DOB", "GradeId")] Student student)
        {
           
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch(DbUpdateException ex)
                {

                }
            }

            ViewData["GradeList"] = new SelectList(_context.Grades, "GradeId", "GradeName", student.GradeId);
            return View(student);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}
