using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Data;
using HajurkoCarRental.Models;

namespace HajurkoCarRental.Controllers
{
    public class SalesController : Controller
    {
        private readonly HajurkoCarRentalContext _context;

        public SalesController(HajurkoCarRentalContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var hajurkoCarRentalContext = _context.Sales.Include(s => s.Car).Include(s => s.Order).ThenInclude(o => o.Users).Include(s => s.HajurkoCarRentalUser);
            return View(await hajurkoCarRentalContext.ToListAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales
                .Include(s => s.Car)
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id");
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id");
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CarId,OrderId,RentalDuration,RentalCharges,ApprovedBy")] Sales sales)
        {
            
                sales.Id = Guid.NewGuid();
                _context.Add(sales);
                await _context.SaveChangesAsync();
             return RedirectToAction("Approve", "Orders", new { id = sales.OrderId });
            
        }

      

        public async Task<IActionResult> RentalHistory(string userId)
        {
            var sales = await _context.Sales
        .Include(s => s.Car)
        .Include(s => s.Order)
        .Where(s => s.Order.Users.Id == userId)
        .ToListAsync();

            return View(sales);
        }


        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales.FindAsync(id);
            if (sales == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id", sales.CarId);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", sales.OrderId);
            return View(sales);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CarId,OrderId,RentalDuration,RentalCharges,ApprovedBy")] Sales sales)
        {
            if (id != sales.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesExists(sales.Id))
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
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id", sales.CarId);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", sales.OrderId);
            return View(sales);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales
                .Include(s => s.Car)
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Sales == null)
            {
                return Problem("Entity set 'HajurkoCarRentalContext.Sales'  is null.");
            }
            var sales = await _context.Sales.FindAsync(id);
            if (sales != null)
            {
                _context.Sales.Remove(sales);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesExists(Guid id)
        {
          return (_context.Sales?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
