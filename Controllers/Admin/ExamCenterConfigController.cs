using CETExamApp.Data;
using CETExamApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ExamCenterConfigController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ExamCenterConfigController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var config = await _context.ExamCenterConfigs.FirstOrDefaultAsync();
            
            if (config == null)
            {
                // Create default config
                config = new ExamCenterConfig
                {
                    CenterName = "Central Exam Testing Center",
                    PrimaryColor = "#007bff",
                    SecondaryColor = "#6c757d"
                };
            }

            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ExamCenterConfig model, IFormFile? logoFile)
        {
            if (ModelState.IsValid)
            {
                var config = await _context.ExamCenterConfigs.FirstOrDefaultAsync();

                // Handle logo upload
                if (logoFile != null && logoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = "logo_" + Guid.NewGuid().ToString() + Path.GetExtension(logoFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(fileStream);
                    }

                    model.LogoPath = "/images/" + uniqueFileName;
                }
                else if (config != null)
                {
                    // Keep existing logo path
                    model.LogoPath = config.LogoPath;
                }

                model.LastUpdated = DateTime.UtcNow;
                model.UpdatedBy = User.Identity?.Name;

                if (config == null)
                {
                    // Create new
                    _context.ExamCenterConfigs.Add(model);
                }
                else
                {
                    // Update existing
                    model.Id = config.Id;
                    _context.Entry(config).CurrentValues.SetValues(model);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Exam Center configuration updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View("Index", model);
        }
    }
}

