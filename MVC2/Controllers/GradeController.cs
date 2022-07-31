using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC2.Helper;
using MVC2.Models;
using MVC2.ViewModels;

namespace MVC2.Controllers
{
    public class GradeController : Controller
    {
        private readonly DatabaseContext _context;

        public GradeController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {

            ViewData["nameS"] = sortOrder == "Name" ? "NameD" : "Name";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var grade = await _context.Grades.Include(g => g.SubjectsTaught)
                            .ThenInclude(s => s.Subject).AsNoTracking().ToListAsync();



            if (!String.IsNullOrEmpty(searchString))
            {
                grade = grade.Where(s => s.GradeName.Contains(searchString)).ToList();
            }


            switch (sortOrder)
            {
                case "Name":
                    grade = grade.OrderBy(s => s.GradeName).ToList();
                    break;
                case "NameD":
                    grade = grade.OrderByDescending(s => s.GradeName).ToList();
                    break;
                default:
                    break;
            }


            int pageSize = 3;
            return View(await PaginatedList<Grade>.CreateAsync(grade, pageNumber ?? 1, pageSize));
        }

        public IActionResult Create()
        {
            ViewData["Subject"] = _context.Subjects.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GradeName")] Grade grade, string[] subjectIndex)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(grade);
                    await _context.SaveChangesAsync();

                    foreach(var i in subjectIndex)
                    {
                        var sG = new GradeSubject();
                        sG.SubjectId = Int32.Parse(i);
                        sG.GradeId = grade.GradeId;
                        _context.Add(sG);
                    }
                    await _context.SaveChangesAsync();


                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                }

            }
            ViewData["Subject"] = _context.Subjects.ToList();
            return View(grade);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = await _context.Grades.Include(s=>s.SubjectsTaught).Where(g=>g.GradeId == id).FirstAsync();
            if (grade == null)
            {
                return NotFound();
            }

            ViewData["Subject"] = _context.Subjects.ToList();
            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("GradeId", "GradeName")] Grade grade, string[] subjectIndex)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grade);
                    await _context.SaveChangesAsync();


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
                    //add grade subject

                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {

                }
            }

            ViewData["Subject"] = _context.Subjects.ToList();
            return View(grade);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                //remove grade subjects
                var gS = _context.GradeSubjects.Where(g => g.GradeId == grade.GradeId);
                _context.RemoveRange(gS);

                _context.Grades.Remove(grade);

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
