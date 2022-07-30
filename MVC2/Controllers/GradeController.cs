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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectName")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(subject);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                }

            }
            return View(subject);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("SubjectId", "SubjectName")] Subject subject)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {

                }
            }

            return View(subject);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Subjects.Remove(subject);
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
