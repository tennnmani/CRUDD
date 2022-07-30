using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC2.Helper;
using MVC2.Models;
using MVC2.ViewModels;

namespace MVC2.Controllers
{
    public class SubjectController : Controller
    {
        private readonly DatabaseContext _context;

        public SubjectController(DatabaseContext context)
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

            var subjects = (from s in _context.Subjects
                            select s).AsNoTracking();

            var subjectClass = new List<SubjectClassVM>();

            foreach (var s in subjects)
            {
                var subc = new SubjectClassVM();
                subc.SubjectId = s.SubjectId;
                subc.SubjectName = s.SubjectName;
                subc.Grade = (from g in _context.Grades
                              join gs in _context.GradeSubjects on g.GradeId equals gs.GradeId
                              where gs.SubjectId == s.SubjectId
                              select g).AsNoTracking().ToList();

                subjectClass.Add(subc);
            }


            if (!String.IsNullOrEmpty(searchString))
            {
                subjectClass = subjectClass.Where(s => s.SubjectName.Contains(searchString)).ToList();
            }


            switch (sortOrder)
            {
                case "Name":
                    subjectClass = subjectClass.OrderBy(s => s.SubjectName).ToList();
                    break;
                case "NameD":
                    subjectClass = subjectClass.OrderByDescending(s => s.SubjectName).ToList();
                    break;
                default:
                    break;
            }


            int pageSize = 3;
            return View(await PaginatedList<SubjectClassVM>.CreateAsync(subjectClass, pageNumber ?? 1, pageSize));
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
        public async Task<IActionResult> Edit([Bind("SubjectId","SubjectName")] Subject subject)
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
