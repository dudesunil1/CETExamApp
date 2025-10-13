using CETExamApp.Data;
using CETExamApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
            return View(classes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Class @class)
        {
            if (ModelState.IsValid)
            {
                @class.CreatedDate = DateTime.UtcNow;
                _context.Classes.Add(@class);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();

            return View(@class);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Class @class)
        {
            if (id != @class.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Class updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ClassExists(@class.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @class = await _context.Classes
                .Include(c => c.Groups)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null) return NotFound();

            return View(@class);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ClassExists(int id)
        {
            return await _context.Classes.AnyAsync(e => e.Id == id);
        }
    }
}

