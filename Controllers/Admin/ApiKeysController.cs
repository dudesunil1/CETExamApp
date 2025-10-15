using CETExamApp.Data;
using CETExamApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ApiKeysController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApiKeysController> _logger;

        public ApiKeysController(ApplicationDbContext context, ILogger<ApiKeysController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ApiKeys
        public async Task<IActionResult> Index()
        {
            var apiKeys = await _context.ApiKeys
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
            return View(apiKeys);
        }

        // GET: ApiKeys/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApiKeys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiKey model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.UtcNow;
                _context.ApiKeys.Add(model);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "API Key created successfully!";
                _logger.LogInformation("API Key created: {Id}", model.Id);
                
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: ApiKeys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null) return NotFound();

            return View(apiKey);
        }

        // POST: ApiKeys/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApiKey model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var apiKey = await _context.ApiKeys.FindAsync(id);
                    if (apiKey == null) return NotFound();

                    apiKey.ApiKeyValue = model.ApiKeyValue;
                    apiKey.InActive = model.InActive;
                    apiKey.Email = model.Email;
                    apiKey.Password = model.Password;
                    apiKey.Description = model.Description;
                    apiKey.LastModified = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "API Key updated successfully!";
                    _logger.LogInformation("API Key updated: {Id}", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApiKeyExists(model.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: ApiKeys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null) return NotFound();

            return View(apiKey);
        }

        // POST: ApiKeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey != null)
            {
                _context.ApiKeys.Remove(apiKey);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "API Key deleted successfully!";
                _logger.LogInformation("API Key deleted: {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ApiKeyExists(int id)
        {
            return _context.ApiKeys.Any(e => e.Id == id);
        }
    }
}

