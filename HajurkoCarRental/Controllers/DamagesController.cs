using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Data;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Identity;
using HajurkoCarRental.Areas.Identity.Data;

namespace HajurkoCarRental.Controllers
{
    public class DamagesController : Controller
    {
        private readonly HajurkoCarRentalContext _context;
        private readonly UserManager<HajurkoCarRentalUser> _userManager;

        public DamagesController(HajurkoCarRentalContext context, UserManager<HajurkoCarRentalUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Damages
        public async Task<IActionResult> Index()
        {
            var hajurkoCarRentalContext = _context.Damages.Include(d => d.Car);
            return View(await hajurkoCarRentalContext.ToListAsync());
        }

        // GET: Damages/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Damages == null)
            {
                return NotFound();
            }

            var damages = await _context.Damages
                .Include(d => d.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (damages == null)
            {
                return NotFound();
            }

            return View(damages);
        }

        // GET: Damages/Create
        public async Task<IActionResult> Create(Guid id)
        {
            var car = await _context.Car.FirstOrDefaultAsync(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            var damage = new Damages { CarId = car.Id, UserId=_userManager.GetUserId(User) };
            return View(damage);

        }

        // POST: Damages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CarId,DamageDesc,RepairCost,Status,UserId")] Damages damages)
        {
            
                damages.Id = Guid.NewGuid();
                _context.Add(damages);
                await _context.SaveChangesAsync();
            TempData["Message"] = "Your Report has been submitted.";
                return RedirectToAction("Index","Home");
       
        }

        // GET: Damages/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Damages == null)
            {
                return NotFound();
            }

            var damages = await _context.Damages.FindAsync(id);
            if (damages == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Id", damages.CarId);
            return View(damages);
        }

        // POST: Damages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CarId,DamageDesc,RepairCost,Status,UserId")] Damages damages)
        {
            if (id != damages.Id)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(damages);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DamagesExists(damages.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            var notification = new Notifications
            {
                Id = Guid.NewGuid(),
                To = damages.UserId,
                NotificationType = "Damage Report Charged Rs " + damages.RepairCost
            };
            _context.Add(notification);
            await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
           
        }

        // GET: Damages/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Damages == null)
            {
                return NotFound();
            }

            var damages = await _context.Damages
                .Include(d => d.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (damages == null)
            {
                return NotFound();
            }

            return View(damages);
        }

        // POST: Damages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Damages == null)
            {
                return Problem("Entity set 'HajurkoCarRentalContext.Damages'  is null.");
            }
            var damages = await _context.Damages.FindAsync(id);
            if (damages != null)
            {
                _context.Damages.Remove(damages);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DamagesExists(Guid id)
        {
          return (_context.Damages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
