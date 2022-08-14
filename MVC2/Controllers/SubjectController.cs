using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC2.Models;
using MVC2.Interface;

namespace MVC2.Controllers
{
    public class SubjectController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IPagination _paginationinfo;
        private readonly ISubject _subjectinfo;

        public SubjectController(DatabaseContext context, IPagination paginationInfo, ISubject subjectinfo)
        {
            _context = context;
            _paginationinfo = paginationInfo;
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
                pageNum = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            var subjects = _subjectinfo.getFiltteredSubject(searchString, fromDate, toDate);

            int ps = _paginationinfo.pageSize(pageSize);
            if (ps == 0)
            {
                ps = _subjectinfo.count();
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentFilter"] = searchString;
            ViewData["FromDate"] = fromDate;
            ViewData["ToDate"] = toDate;
            ViewData["PageSize"] = ps;

            ViewData["Pagination"] = new Pagination()
            {
                PageSize = ps,
                TotalPage = (int)Math.Ceiling(subjects.Count() / (double)ps),
                PageIndex = pageNum,
            };

            
            //ViewData["TotalPage"] = (int)Math.Ceiling(subjects.Count() / (double)ps);
            //ViewData["PageIndex"] = pageNum;

            var display = await subjects.Skip((pageNum - 1) * ps).Take(ps).ToListAsync();
            return View(display);

           // return View(await PaginatedList<Subject>.CreateAsync(subjects, pageNumber ?? 1, ps));
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
                await _subjectinfo.createSubject(subject);
                return RedirectToAction("Index");

            }
            return View(subject);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _subjectinfo.getSubject(id);
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
                await _subjectinfo.updateSubject(subject);
                return RedirectToAction("Index");
            }

            return View(subject);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _subjectinfo.getSubject(id);
            if (subject == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await _subjectinfo.removeSubject(subject);

            return RedirectToAction(nameof(Index));
        }
    }
}
