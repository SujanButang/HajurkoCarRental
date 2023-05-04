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
            var carsInOffer = await (
     from co in _context.CarOffer
     join c in _context.Car on co.CarID equals c.Id
     join o in _context.Offers on co.Offer equals o.Id // join with Offers
     select new Car
     {
         Id = c.Id,
         Name = c.Name,
         Model = c.Model,
         Year = c.Year,
         DailyRent = c.DailyRent,
         Status = c.Status,
         PhotoUrl = c.PhotoUrl,
         Offers = new List<OffersViewModel> // populate the Offers list
         {
            new OffersViewModel
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                DiscountPercentage = o.DiscountPercentage,
                Validity = o.Validity
            }
         }
     }
 ).ToListAsync();

            var carIdsInOffer = carsInOffer.Select(co => co.Id);
            var carsNotInOffer = await _context.Car.Where(c => !carIdsInOffer.Contains(c.Id)).ToListAsync();


            var orders = await _context.Order.Where(o => o.Status == "Approved").ToListAsync();



            var offerCarsNotInOrders = carsInOffer.Where(c => !orders.Any(o => o.CarId == c.Id && DateTime.Now >= o.OrderDate && DateTime.Now <= o.ReturnDate)).ToList();
            var normalCarsNotInOrders = carsNotInOffer.Where(c => !orders.Any(o => o.CarId == c.Id && DateTime.Now >= o.OrderDate && DateTime.Now <= o.ReturnDate)).ToList();

            var offerCarsInOrders = carsInOffer.Where(c => orders.Any(o => o.CarId == c.Id && DateTime.Now >= o.OrderDate && DateTime.Now <= o.ReturnDate)).ToList();
            var normalCarsInOrders = carsNotInOffer.Where(c => orders.Any(o => o.CarId == c.Id && DateTime.Now >= o.OrderDate && DateTime.Now <= o.ReturnDate)).ToList();

            if (offerCarsInOrders.Any())
            {
                foreach (var car in offerCarsInOrders)
                {
                    var rentedCar = await _context.Car.FirstAsync(c => c.Id == car.Id);
                    rentedCar.Status = "Rented";
                    _context.SaveChangesAsync();
                }
            }
            if (normalCarsInOrders.Any())
            {
                foreach (var car in normalCarsInOrders)
                {
                    var rentedCar = await _context.Car.FirstAsync(c => c.Id == car.Id);
                    rentedCar.Status = "Rented";
                    _context.SaveChangesAsync();
                }
            }


            var allCars = new CarViewModel
            {
                CarsInOffers = offerCarsNotInOrders,
                CarsNotInOffers = normalCarsNotInOrders
            };

            var carViewModels = new List<CarViewModel> { allCars };

            return View(carViewModels);
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