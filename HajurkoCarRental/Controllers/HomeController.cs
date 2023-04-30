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
            var carIdsInOffers = await _context.CarOffer.Select(co => co.CarID).ToListAsync();
            var cars = await _context.Car.Where(c => !carIdsInOffers.Contains(c.Id)).ToListAsync();
            var carsNotInOffer = await _context.Car.Where(c => carIdsInOffers.Contains(c.Id)).ToListAsync();

            var allCars = new CarViewModel
            {
                CarsInOffers = cars,
                CarsNotInOffers = carsNotInOffer
            };

            var carViewModels = new List<CarViewModel>();
            carViewModels.Add(allCars);

            return View(carViewModels);
        }

        public async Task<IActionResult> Offers()
        {
            var carIdsInOffers = await _context.CarOffer.Select(co => co.CarID).ToListAsync();
            var cars = await _context.Car.Where(c => carIdsInOffers.Contains(c.Id)).ToListAsync();

            return View(cars);
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