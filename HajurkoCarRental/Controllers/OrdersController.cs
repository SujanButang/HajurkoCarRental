using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Data;
using HajurkoCarRental.Models;
using HajurkoCarRental.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HajurkoCarRental.Controllers
{
    public class OrdersController : Controller
    {
        private readonly HajurkoCarRentalContext _context;


        public OrdersController(HajurkoCarRentalContext context)
        {
            _context = context;
        }


        [Authorize]
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            


            var hajurkoCarRentalContext = _context.Order.Include(o => o.Car).Include(o=>o.Users);
            return View(await hajurkoCarRentalContext.ToListAsync());
            }
        

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Car)
                .Include(o=>o.Users)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [Authorize]
        // GET: Orders/Create
        public IActionResult Create(string carId)
        {
            ViewBag.CarId = carId;
            // rest of the code
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            
                order.Id = Guid.NewGuid();
                _context.Add(order);
                await _context.SaveChangesAsync();

            TempData["Message"] = "Order added successfully!";
            return RedirectToAction("Index","Home");           
  
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        
        public async Task<IActionResult> Approve(Guid id)
        {
            
            var order = await _context.Order.FindAsync(id);
            if (order==null)
            {
                return NotFound();
            }
            order.Status = "Approved";
            await _context.SaveChangesAsync();

             var notification = new Notifications
    {
        Id = Guid.NewGuid(),
        To = order.CustomerId,
        NotificationType = "Your Rental Request for "+ order.Car.Name + order.Car.Model +"has been approved by the system."
    };

    // Add the notification to the context and save changes
    _context.Add(notification);
    await _context.SaveChangesAsync();

    // Redirect to the Index action of the Orders controller
    return RedirectToAction("Index", "Orders");
        }

        

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CarId,CustomerId,OrderDate,ReturnDate,Status")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id", order.CarId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'HajurkoCarRentalContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(Guid id)
        {
          return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
