using CETExamApp.Data;
using CETExamApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TopicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var topics = await _context.Topics
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
            return View(topics);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name");
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Topic topic)
        {
            if (ModelState.IsValid)
            {
                topic.CreatedDate = DateTime.UtcNow;
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Topic created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name", topic.SubjectId);
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", topic.ClassId);
            return View(topic);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound();

            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name", topic.SubjectId);
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", topic.ClassId);
            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Topic topic)
        {
            if (id != topic.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Topic updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TopicExists(topic.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name", topic.SubjectId);
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", topic.ClassId);
            return View(topic);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var topic = await _context.Topics
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null) return NotFound();

            return View(topic);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Topic deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TopicExists(int id)
        {
            return await _context.Topics.AnyAsync(e => e.Id == id);
        }
    }
}

