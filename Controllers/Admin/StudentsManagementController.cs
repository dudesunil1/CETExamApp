using CETExamApp.Data;
using CETExamApp.Models;
using CETExamApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class StudentsManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public StudentsManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var studentsWithDetails = students.Select(s => new
            {
                Student = s,
                Class = _context.Classes.FirstOrDefault(c => c.Id == s.ClassId),
                Group = _context.Groups.FirstOrDefault(g => g.Id == s.GroupId)
            }).ToList();

            return View(studentsWithDetails);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle photo upload
                string? photoPath = null;
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    photoPath = await SavePhotoAsync(model.Photo);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    StudentUsername = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    ClassId = model.ClassId,
                    GroupId = model.GroupId,
                    PhotoPath = photoPath,
                    MobileNo = model.MobileNo,
                    ParentsMobileNo = model.ParentsMobileNo,
                    EmailConfirmed = true,
                    PhoneNumber = model.MobileNo
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Auto-assign Student role
                    await _userManager.AddToRoleAsync(user, "Student");
                    TempData["Success"] = "Student registered successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", model.ClassId);
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name", model.GroupId);
            return View(model);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var student = await _userManager.FindByIdAsync(id);
            if (student == null) return NotFound();

            var model = new StudentEditViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName ?? "",
                LastName = student.LastName ?? "",
                Username = student.StudentUsername ?? "",
                Email = student.Email ?? "",
                ClassId = student.ClassId ?? 0,
                GroupId = student.GroupId ?? 0,
                CurrentPhotoPath = student.PhotoPath,
                MobileNo = student.MobileNo ?? "",
                ParentsMobileNo = student.ParentsMobileNo ?? "",
                IsActive = student.IsActive
            };

            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", student.ClassId);
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name", student.GroupId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, StudentEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var student = await _userManager.FindByIdAsync(id);
                if (student == null) return NotFound();

                // Handle photo upload
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    // Delete old photo if exists
                    if (!string.IsNullOrEmpty(student.PhotoPath))
                    {
                        DeletePhoto(student.PhotoPath);
                    }
                    student.PhotoPath = await SavePhotoAsync(model.Photo);
                }

                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                student.StudentUsername = model.Username;
                student.Email = model.Email;
                student.UserName = model.Username;
                student.ClassId = model.ClassId;
                student.GroupId = model.GroupId;
                student.MobileNo = model.MobileNo;
                student.ParentsMobileNo = model.ParentsMobileNo;
                student.PhoneNumber = model.MobileNo;
                student.IsActive = model.IsActive;

                var result = await _userManager.UpdateAsync(student);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Student updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", model.ClassId);
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name", model.GroupId);
            return View(model);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var student = await _userManager.FindByIdAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student != null)
            {
                // Delete photo if exists
                if (!string.IsNullOrEmpty(student.PhotoPath))
                {
                    DeletePhoto(student.PhotoPath);
                }
                
                await _userManager.DeleteAsync(student);
                TempData["Success"] = "Student deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to save photo
        private async Task<string> SavePhotoAsync(IFormFile photo)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "students");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            return "/uploads/students/" + uniqueFileName;
        }

        // Helper method to delete photo
        private void DeletePhoto(string photoPath)
        {
            if (!string.IsNullOrEmpty(photoPath))
            {
                var fullPath = Path.Combine(_environment.WebRootPath, photoPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
    }
}

