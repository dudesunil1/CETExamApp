using CETExamApp.Data;
using CETExamApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups
                .Include(g => g.Class)
                .OrderByDescending(g => g.CreatedDate)
                .ToListAsync();
            return View(groups);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group group)
        {
            if (ModelState.IsValid)
            {
                group.CreatedDate = DateTime.UtcNow;
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Group created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", group.ClassId);
            return View(group);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();

            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", group.ClassId);
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Group group)
        {
            if (id != group.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(group);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Group updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GroupExists(group.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", group.ClassId);
            return View(group);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var group = await _context.Groups
                .Include(g => g.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null) return NotFound();

            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Group deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> GroupExists(int id)
        {
            return await _context.Groups.AnyAsync(e => e.Id == id);
        }
    }
}

