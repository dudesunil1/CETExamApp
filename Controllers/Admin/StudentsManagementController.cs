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
                    Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email,
                    StudentUsername = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    ClassId = model.ClassId,
                    GroupId = model.GroupId,
                    PhotoPath = photoPath,
                    MobileNo = model.MobileNo,
                    ParentsMobileNo = string.IsNullOrWhiteSpace(model.ParentsMobileNo) ? null : model.ParentsMobileNo,
                    Gender = model.Gender,
                    EmailConfirmed = !string.IsNullOrWhiteSpace(model.Email),
                    PhoneNumber = model.MobileNo
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Auto-assign Student role
                    await _userManager.AddToRoleAsync(user, "Student");
                    TempData["Success"] = $"Student registered successfully! Username: {model.Username}, Password: {model.Password}";
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
                Gender = student.Gender ?? "Male",
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
                student.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email;
                student.UserName = model.Username;
                student.ClassId = model.ClassId;
                student.GroupId = model.GroupId;
                student.MobileNo = model.MobileNo;
                student.ParentsMobileNo = string.IsNullOrWhiteSpace(model.ParentsMobileNo) ? null : model.ParentsMobileNo;
                student.Gender = model.Gender;
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

        public async Task<IActionResult> ChangePassword(string? id)
        {
            if (id == null) return NotFound();

            var student = await _userManager.FindByIdAsync(id);
            if (student == null) return NotFound();

            var model = new ChangePasswordViewModel
            {
                Id = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}".Trim()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var student = await _userManager.FindByIdAsync(id);
                if (student == null) return NotFound();

                // Remove the old password and set the new one
                var removePasswordResult = await _userManager.RemovePasswordAsync(student);
                if (removePasswordResult.Succeeded)
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(student, model.NewPassword);
                    if (addPasswordResult.Succeeded)
                    {
                        TempData["Success"] = $"Password changed successfully for {model.StudentName}!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in addPasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    foreach (var error in removePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

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

        // Bulk Upload Students
        [HttpGet]
        public async Task<IActionResult> BulkUpload()
        {
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name");
            return View(new BulkStudentUploadViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpload(BulkStudentUploadViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file to upload.");
                ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
                ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name");
                return View(model);
            }

            try
            {
                var result = await ProcessBulkUpload(model.File);
                model.UploadResult = result;

                if (result.SuccessfulUploads > 0)
                {
                    TempData["Success"] = $"Successfully uploaded {result.SuccessfulUploads} students. {result.FailedUploads} failed.";
                }
                else
                {
                    TempData["Error"] = "No students were uploaded successfully.";
                }

                ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
                ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name");
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred during bulk upload: {ex.Message}";
                ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
                ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name");
                return View(model);
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Route("StudentsManagement/PreviewBulkUpload")]
        public async Task<IActionResult> PreviewBulkUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Please select a file to upload." });
            }

            // Check file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (fileExtension != ".csv")
            {
                return Json(new { success = false, message = "Please upload a CSV file." });
            }

            // Check file size (limit to 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "File size must be less than 5MB." });
            }

            try
            {
                var previewData = await ParseCsvFile(file);
                
                // Log preview data for debugging
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<StudentsManagementController>>();
                foreach (var student in previewData.Take(3)) // Log first 3 students for debugging
                {
                    logger.LogInformation("Preview - Student: {StudentName}, ClassId: {ClassId}, GroupId: {GroupId}, ClassName: {ClassName}, GroupName: {GroupName}", 
                        student.StudentName, student.ClassId, student.GroupId, student.ClassName, student.GroupName);
                }
                
                return Json(new { success = true, data = previewData });
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<StudentsManagementController>>();
                logger.LogError(ex, "Error parsing CSV file: {FileName}", file.FileName);
                
                return Json(new { 
                    success = false, 
                    message = $"Error parsing file: {ex.Message}",
                    details = ex.InnerException?.Message ?? ex.StackTrace
                });
            }
        }

        private async Task<List<StudentUploadRow>> ParseCsvFile(IFormFile file)
        {
            var students = new List<StudentUploadRow>();
            var classes = await _context.Classes.ToListAsync();
            var groups = await _context.Groups.Include(g => g.Class).ToListAsync();

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string? line;
                    int rowNumber = 0;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        rowNumber++;
                        if (rowNumber == 1) continue; // Skip header row

                        if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

                        var columns = ParseCsvLine(line);
                        if (columns.Length < 6) 
                        {
                            // Add invalid row to show error
                            var invalidStudent = new StudentUploadRow
                            {
                                RowNumber = rowNumber,
                                StudentName = "Invalid Row",
                                IsValid = false
                            };
                            invalidStudent.Errors.Add($"Row has only {columns.Length} columns, expected at least 6");
                            students.Add(invalidStudent);
                            continue;
                        }

                        var student = new StudentUploadRow
                        {
                            RowNumber = rowNumber,
                            StudentName = columns.Length > 0 ? (columns[0]?.Trim() ?? "") : "",
                            Username = columns.Length > 4 ? (columns[4]?.Trim() ?? "") : "",
                            Password = columns.Length > 5 ? (columns[5]?.Trim() ?? "") : "",
                            MobileNo = columns.Length > 3 ? (columns[3]?.Trim() ?? "") : "",
                            Email = columns.Length > 6 ? (columns[6]?.Trim() ?? "") : "",
                            Gender = columns.Length > 7 ? (columns[7]?.Trim() ?? "Male") : "Male"
                        };

                        // Parse ClassId
                        if (columns.Length > 1 && !string.IsNullOrWhiteSpace(columns[1]))
                        {
                            if (int.TryParse(columns[1]?.Trim(), out int classId) && classId > 0)
                            {
                                student.ClassId = classId;
                                student.ClassName = classes.FirstOrDefault(c => c.Id == classId)?.Name;
                            }
                        }

                        // Parse GroupId
                        if (columns.Length > 2 && !string.IsNullOrWhiteSpace(columns[2]))
                        {
                            if (int.TryParse(columns[2]?.Trim(), out int groupId) && groupId > 0)
                            {
                                student.GroupId = groupId;
                                student.GroupName = groups.FirstOrDefault(g => g.Id == groupId)?.Name;
                            }
                        }

                        // Validate the student data
                        ValidateStudentUploadRow(student);
                        students.Add(student);
                    }
                }
            }
            catch (Exception ex)
            {
                // Add error row if parsing fails completely
                var errorStudent = new StudentUploadRow
                {
                    RowNumber = 0,
                    StudentName = "Parsing Error",
                    IsValid = false
                };
                errorStudent.Errors.Add($"Error parsing CSV file: {ex.Message}");
                students.Add(errorStudent);
            }

            return students;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            result.Add(current);
            return result.ToArray();
        }

        private void ValidateStudentUploadRow(StudentUploadRow student)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(student.StudentName))
                errors.Add("Student name is required");

            if (student.ClassId <= 0)
                errors.Add("Valid Class ID is required");

            if (student.GroupId <= 0)
                errors.Add("Valid Group ID is required");

            if (string.IsNullOrWhiteSpace(student.Username))
                errors.Add("Username is required");

            if (string.IsNullOrWhiteSpace(student.Password) || student.Password.Length < 6)
                errors.Add("Password must be at least 6 characters long");

            if (!string.IsNullOrWhiteSpace(student.Email) && !IsValidEmail(student.Email))
                errors.Add("Invalid email format");

            if (!string.IsNullOrWhiteSpace(student.MobileNo) && !IsValidPhone(student.MobileNo))
                errors.Add("Invalid mobile number format");

            // Validate ClassId exists in database
            if (student.ClassId > 0 && string.IsNullOrWhiteSpace(student.ClassName))
                errors.Add($"Class ID {student.ClassId} does not exist in the system");

            // Validate GroupId exists in database
            if (student.GroupId > 0 && string.IsNullOrWhiteSpace(student.GroupName))
                errors.Add($"Group ID {student.GroupId} does not exist in the system");

            student.Errors = errors;
            student.IsValid = !errors.Any();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?[1-9]\d{1,14}$");
        }

        private async Task<BulkUploadResult> ProcessBulkUpload(IFormFile file)
        {
            var result = new BulkUploadResult();
            var students = await ParseCsvFile(file);
            result.TotalRows = students.Count;

            foreach (var studentData in students.Where(s => s.IsValid))
            {
                try
                {
                    var user = new ApplicationUser
                    {
                        UserName = studentData.Username,
                        Email = studentData.Email ?? $"{studentData.Username}@school.com",
                        FirstName = studentData.StudentName.Split(' ').FirstOrDefault() ?? studentData.StudentName,
                        LastName = studentData.StudentName.Contains(' ') ? string.Join(" ", studentData.StudentName.Split(' ').Skip(1)) : "",
                        StudentUsername = studentData.Username,
                        ClassId = studentData.ClassId,
                        GroupId = studentData.GroupId,
                        MobileNo = studentData.MobileNo,
                        PhoneNumber = studentData.MobileNo,
                        Gender = studentData.Gender,
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    // Log the values being saved for debugging
                    var logger = HttpContext.RequestServices.GetRequiredService<ILogger<StudentsManagementController>>();
                    logger.LogInformation("Creating student: {StudentName}, ClassId: {ClassId}, GroupId: {GroupId}", 
                        studentData.StudentName, studentData.ClassId, studentData.GroupId);

                    var createResult = await _userManager.CreateAsync(user, studentData.Password);
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Student");
                        
                        // Verify the student was created with correct ClassId and GroupId
                        var createdUser = await _userManager.FindByNameAsync(user.UserName);
                        if (createdUser != null)
                        {
                            logger.LogInformation("Student created successfully: {StudentName}, ClassId: {ClassId}, GroupId: {GroupId}", 
                                createdUser.FirstName, createdUser.ClassId, createdUser.GroupId);
                        }
                        
                        result.SuccessfulUploads++;
                        result.SuccessMessages.Add($"Student {studentData.StudentName} created successfully - Username: {studentData.Username}, Password: {studentData.Password}");
                    }
                    else
                    {
                        result.FailedUploads++;
                        result.ErrorMessages.Add($"Failed to create student {studentData.StudentName}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    result.FailedUploads++;
                    result.ErrorMessages.Add($"Error creating student {studentData.StudentName}: {ex.Message}");
                }
            }

            // Add validation errors for invalid rows
            var invalidStudents = students.Where(s => !s.IsValid).ToList();
            result.FailedUploads += invalidStudents.Count;
            foreach (var invalidStudent in invalidStudents)
            {
                result.ErrorMessages.Add($"Row {invalidStudent.RowNumber} ({invalidStudent.StudentName}): {string.Join(", ", invalidStudent.Errors)}");
            }

            return result;
        }
    }
}

