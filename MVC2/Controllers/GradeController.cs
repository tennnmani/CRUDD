using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC2.Interface;
using MVC2.Models;
using MVC2.ViewModels;

namespace MVC2.Controllers
{
    public class GradeController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IPagination _paginationinfo;
        private readonly IGrade _gradeinfo;
        private readonly ISubject _subjectinfo;

        public GradeController(DatabaseContext context, IPagination paginationInfo, IGrade gradeinfo, ISubject subjectinfo)
        {
            _context = context;
            _paginationinfo = paginationInfo;
            _gradeinfo = gradeinfo;
            _subjectinfo = subjectinfo;
        }

        public async Task<IActionResult> Index(
            string currentFilter,
            string searchString,
            int? pageNumber,
            string fromDate,
            string toDate,
            string pageSize)
        {
            int pageNum = pageNumber ?? 1;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var grade = _gradeinfo.getFiltteredGrade(searchString, fromDate, toDate);

            int ps = _paginationinfo.pageSize(pageSize);
            if (ps == 0)
            {
                ps = _gradeinfo.count();
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["FromDate"] = fromDate;
            ViewData["ToDate"] = toDate;
            ViewData["PageSize"] = ps;

            ViewData["Pagination"] = new Pagination()
            {
                PageSize = ps,
                TotalPage = (int)Math.Ceiling(grade.Count() / (double)ps),
                PageIndex = pageNum,
            };

            var display = await grade.Skip((pageNum - 1) * ps).Take(ps).ToListAsync();
            return View(display);

            //return View(await PaginatedList<Grade>.CreateAsync(grade, pageNumber ?? 1, ps));
        }

        public IActionResult Create()
        {
            ViewData["Subject"] = _subjectinfo.getAllSubject();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GradeName")] Grade grade, string[] subjectIndex)
        {
            if (ModelState.IsValid)
            {
                await _gradeinfo.createGrade(grade, subjectIndex);

                return RedirectToAction("Index");

            }
            ViewData["Subject"] = _subjectinfo.getAllSubject();
            return View(grade);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = await _gradeinfo.getGradeWSubject(id);
            if (grade == null)
            {
                return NotFound();
            }

            ViewData["Subject"] = _subjectinfo.getAllSubject();
            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("GradeId", "GradeName")] Grade grade, string[] subjectIndex)
        {

            if (ModelState.IsValid)
            {
                await _gradeinfo.updateGradeAsync(grade, subjectIndex);
                return RedirectToAction("Index");
            }

            ViewData["Subject"] = _subjectinfo.getAllSubject();
            return View(grade);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var grade = await _gradeinfo.getGrade(id);
            if (grade == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await _gradeinfo.removeGrade(grade);
            return RedirectToAction(nameof(Index));
        }
    }
}
