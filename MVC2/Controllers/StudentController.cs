using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC2.Helper;
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

        public StudentController(DatabaseContext context, IPagination paginationInfo, IStudent studentinfo, IGrade gradeinfo)
        {
            _context = context;
            _paginationinfo = paginationInfo;
            _studentinfo = studentinfo;
            _gradeinfo = gradeinfo;
        }

        public async Task<IActionResult> Index(
            string currentFilter,
            string searchString,
            int? pageNumber,
            string fromDate,
            string toDate,
            string pageSize)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


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

            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, ps));
        }


        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentinfo.getStudentWGradeNSub(id);

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
