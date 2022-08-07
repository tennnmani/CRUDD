using Microsoft.AspNetCore.Mvc;
using MVC2.Interface;
using MVC2.Models;
using Newtonsoft.Json;
using System.Net;

namespace MVC2.Controllers
{
    public class ExchangeController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IExchange _exchangeinfo;

        public ExchangeController(DatabaseContext context, IExchange exchangeinfo)
        {
            _context = context;
            _exchangeinfo = exchangeinfo;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _exchangeinfo.callExchange());
        }


        public async Task<JsonResult> GetRates(string fromRate, string toRate, string buySell)
        {
            return Json(await _exchangeinfo.getRate(fromRate,toRate,buySell));
        }
    }
}
