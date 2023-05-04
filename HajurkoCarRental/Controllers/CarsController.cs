using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Data;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Authorization;

namespace HajurkoCarRental.Controllers
{
    public class CarsController : Controller
    {
        private readonly HajurkoCarRentalContext _context;
        private readonly IWebHostEnvironment _webHost;


        public CarsController(HajurkoCarRentalContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // GET: Cars
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
              return _context.Car != null ? 
                          View(await _context.Car.ToListAsync()) :
                          Problem("Entity set 'HajurkoCarRentalContext.Car'  is null.");
        }
        public async Task<IActionResult> RentalCars()
        {
            var carIdsInOffers = await _context.CarOffer.Select(co => co.CarID).ToListAsync();
            var cars = await _context.Car.Where(c => !carIdsInOffers.Contains(c.Id)).ToListAsync();

            return View(cars);
        }



        // GET: Cars/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            var offerList = _context.Offers.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title }).ToList();
            ViewData["OffersList"] = offerList;

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        public async Task<IActionResult> CarDetails(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            var carOffer = await _context.CarOffer
                .Include(co => co.Offers)
                .FirstOrDefaultAsync(co => co.CarID == id);

            if (carOffer != null)
            {
                car.Offers = new List<OffersViewModel>
        {
            new OffersViewModel
            {
                Id = carOffer.Offers.Id,
                Title = carOffer.Offers.Title,
                Description = carOffer.Offers.Description,
                DiscountPercentage = carOffer.Offers.DiscountPercentage
            }
        };
            }

            return View(car);

        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Car car)
        {
            string uniqueFileName = GetProfilePhotoFileName(car);
            car.PhotoUrl = uniqueFileName;

            _context.Add(car);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        //public async Task<IActionResult> Create([Bind("Id,Name,Model,Color,Year,RegistrationNo,DailyRent,Status,PhotoUrl")] Car car)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        car.Id = Guid.NewGuid();
        //        _context.Add(car);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(car);
        //}

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Model,Color,Year,RegistrationNo,DailyRent,Status,PhotoUrl")] Car car)
        {
           
                    _context.Update(car);
                    await _context.SaveChangesAsync();
               
                return RedirectToAction(nameof(Index));
       
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Car == null)
            {
                return Problem("Entity set 'HajurkoCarRentalContext.Car'  is null.");
            }
            var car = await _context.Car.FindAsync(id);
            if (car != null)
            {
                _context.Car.Remove(car);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(Guid id)
        {
          return (_context.Car?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> CarHistory()
        {
            var carCount = await _context.Order
        .Include(o => o.Car)
        .GroupBy(o => new { o.CarId, o.Car.Name, o.Car.Model, o.Car.PhotoUrl })
        .Select(g => new CarHistoryModel
        {
            CarId = g.Key.CarId,
            CarMake = g.Key.Name,
            CarModel = g.Key.Model,
            CarPhoto = g.Key.PhotoUrl,
            NumOrders = g.Count()
        })
        .ToListAsync();

            return View(carCount);
        }

        private string GetProfilePhotoFileName(Car car)
        {
            string uniqueFileName = null;

            if (car.Photo != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + car.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    car.Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
