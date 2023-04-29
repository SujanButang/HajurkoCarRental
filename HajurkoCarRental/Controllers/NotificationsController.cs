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
    public class NotificationsController : Controller
    {
        private readonly HajurkoCarRentalContext _context;
        private readonly UserManager<HajurkoCarRentalUser> _userManager;

        public NotificationsController(HajurkoCarRentalContext context, UserManager<HajurkoCarRentalUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications = await _context.Notifications
                .Include(n => n.Users)
                .Where(n => n.To == user.Id)
                .ToListAsync();
            return View(notifications);
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications
                .Include(n => n.Users)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notifications == null)
            {
                return NotFound();
            }

            return View(notifications);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            ViewData["To"] = new SelectList(_context.Set<Users>(), "Id", "Id");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,To,NotificationType")] Notifications notifications)
        {
            if (ModelState.IsValid)
            {
                notifications.Id = Guid.NewGuid();
                _context.Add(notifications);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["To"] = new SelectList(_context.Set<Users>(), "Id", "Id", notifications.To);
            return View(notifications);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications.FindAsync(id);
            if (notifications == null)
            {
                return NotFound();
            }
            ViewData["To"] = new SelectList(_context.Set<Users>(), "Id", "Id", notifications.To);
            return View(notifications);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,To,NotificationType")] Notifications notifications)
        {
            if (id != notifications.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notifications);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationsExists(notifications.Id))
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
            ViewData["To"] = new SelectList(_context.Set<Users>(), "Id", "Id", notifications.To);
            return View(notifications);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notifications = await _context.Notifications
                .Include(n => n.Users)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notifications == null)
            {
                return NotFound();
            }

            return View(notifications);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Notifications == null)
            {
                return Problem("Entity set 'HajurkoCarRentalContext.Notifications'  is null.");
            }
            var notifications = await _context.Notifications.FindAsync(id);
            if (notifications != null)
            {
                _context.Notifications.Remove(notifications);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationsExists(Guid id)
        {
          return (_context.Notifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
