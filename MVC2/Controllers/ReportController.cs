using Microsoft.AspNetCore.Mvc;
using MVC2.Interface;

namespace MVC2.Controllers
{
    public class ReportController : Controller
    {
        public readonly IReport _reporotinfo;
        public ReportController( IReport reportinfo)
        {
            _reporotinfo = reportinfo;
        }
        public IActionResult Index()
        {

            ViewData["Min"] = _reporotinfo.gradeAgeMin();
            ViewData["Max"] = _reporotinfo.gradeAgeMax();
            ViewData["Average"] = _reporotinfo.gradeAgeAvrage();
            ViewData["Sum"] = _reporotinfo.gradeAgeSum();

            return View(_reporotinfo.gradeAge());
        }

        public IActionResult JoinedCount()
        {
            return View(_reporotinfo.getJoinedDateCount());
        }
    }
}
