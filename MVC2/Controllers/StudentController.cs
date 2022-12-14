using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MVC2.Interface;
using MVC2.Models;

namespace MVC2.Controllers
{
    public class StudentController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IPagination _paginationinfo;
        private readonly IStudent _studentinfo;
        private readonly IGrade _gradeinfo;

        private readonly ILogger<StudentController> _logger;

        public StudentController(DatabaseContext context, IPagination paginationInfo, IStudent studentinfo, IGrade gradeinfo, ILogger<StudentController> logger)
        {
            _context = context;
            _paginationinfo = paginationInfo;
            _studentinfo = studentinfo;
            _gradeinfo = gradeinfo;
            _logger = logger;
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

            _logger.LogInformation("text adjfhliasb isjafb iasdf bsiadfbalidfb");
            var students = _studentinfo.getFiltteredStudent(searchString, fromDate, toDate);

            int ps = _paginationinfo.pageSize(pageSize);

            if (ps == 0)
            {
                ps = _studentinfo.count();
            }
            ViewData["CurrentFilter"] = searchString;
            ViewData["FromDate"] = fromDate;
            ViewData["ToDate"] = toDate;
            ViewData["PageSize"] = ps;

            ViewData["Pagination"] = new Pagination()
            {
                PageSize = ps,
                TotalPage = (int)Math.Ceiling(students.Count() / (double)ps),
                PageIndex = pageNum,
            };

            var display = await students.Skip((pageNum - 1) * ps).Take(ps).ToListAsync();
            return View(display);

            //return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, ps));
        }


        public async Task<IActionResult> Details(int id , string name)
        {
            var student = await _studentinfo.getStudentWGradeNSub(id, name);

            return View(student);
        }
        public IActionResult Create()
        {
            ViewData["GradeList"] = new SelectList(_gradeinfo.getAllGrade(), "GradeId", "GradeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName", "LastName", "DOB", "GradeId")] Student student)
        {
            if (ModelState.IsValid)
            {
                await _studentinfo.createStudent(student);
                return RedirectToAction("Index");

            }
            ViewData["GradeList"] = new SelectList(_gradeinfo.getAllGrade(), "GradeId", "GradeName");
            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _studentinfo.getStudent(id);
            if (student == null)
            {
                return NotFound();
            }

            ViewData["GradeList"] = new SelectList(_gradeinfo.getAllGrade(), "GradeId", "GradeName", student.GradeId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("StudentId", "FirstName", "LastName", "DOB", "GradeId")] Student student)
        {

            if (ModelState.IsValid)
            {
                await _studentinfo.updateStudentAsync(student);

                return RedirectToAction("Index");
            }

            ViewData["GradeList"] = new SelectList(_gradeinfo.getAllGrade(), "GradeId", "GradeName", student.GradeId);
            return View(student);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentinfo.getStudent(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await _studentinfo.removeStudent(student);

            return RedirectToAction(nameof(Index));
        }
    }
}
