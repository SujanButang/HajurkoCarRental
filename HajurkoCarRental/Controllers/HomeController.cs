using HajurkoCarRental.Data;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HajurkoCarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HajurkoCarRentalContext _context;

        public HomeController(ILogger<HomeController> logger, HajurkoCarRentalContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.Message = TempData["Message"] as string;
            return _context.Car != null ?
                          View(await _context.Car.ToListAsync()) :
                          Problem("Entity set 'HajurkoCarRentalContext.Car'  is null.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}