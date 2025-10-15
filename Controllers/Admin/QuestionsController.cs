using CETExamApp.Data;
using CETExamApp.Models;
using CETExamApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ITinyMceService _tinyMceService;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(ApplicationDbContext context, IWebHostEnvironment environment, ITinyMceService tinyMceService, ILogger<QuestionsController> logger)
        {
            _context = context;
            _environment = environment;
            _tinyMceService = tinyMceService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? classId, int? subjectId, int? topicId, int? difficultyLevel)
        {
            var questionsQuery = _context.Questions
                .Include(q => q.Topic)
                .ThenInclude(t => t!.Subject)
                .Include(q => q.Topic)
                .ThenInclude(t => t!.Class)
                .AsQueryable();

            // Apply filters
            if (classId.HasValue)
                questionsQuery = questionsQuery.Where(q => q.Topic.ClassId == classId.Value);

            if (subjectId.HasValue)
                questionsQuery = questionsQuery.Where(q => q.Topic.SubjectId == subjectId.Value);

            if (topicId.HasValue)
                questionsQuery = questionsQuery.Where(q => q.TopicId == topicId.Value);

            if (difficultyLevel.HasValue)
                questionsQuery = questionsQuery.Where(q => (int)q.DifficultyLevel == difficultyLevel.Value);

            var questions = await questionsQuery
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();

            // Populate filter dropdowns
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", classId);
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name", subjectId);
            ViewData["TopicId"] = new SelectList(await _context.Topics.Include(t => t.Subject).ToListAsync(), "Id", "Name", topicId);
            ViewData["DifficultyLevel"] = new SelectList(Enum.GetValues(typeof(DifficultyLevel)), difficultyLevel);

            // Store selected values for AJAX
            ViewBag.SelectedClassId = classId;
            ViewBag.SelectedSubjectId = subjectId;
            ViewBag.SelectedTopicId = topicId;
            ViewBag.SelectedDifficultyLevel = difficultyLevel;

            return View(questions);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ClassId"] = new SelectList(await _context.Classes.ToListAsync(), "Id", "Name");
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name");
            ViewData["TopicId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.TinyMceApiKey = await _tinyMceService.GetActiveApiKeyAsync();
            
            // Pass selected values from TempData to ViewBag for maintaining selection
            ViewBag.SelectedClassId = TempData["SelectedClassId"];
            ViewBag.SelectedSubjectId = TempData["SelectedSubjectId"];
            ViewBag.SelectedTopicId = TempData["SelectedTopicId"];
            
            return View();
        }

        // AJAX endpoint to get subjects by class
        [HttpGet]
        public async Task<IActionResult> GetSubjectsByClass(int classId)
        {
            try
            {
                var subjects = await _context.Topics
                    .Where(t => t.ClassId == classId && t.IsActive)
                    .Include(t => t.Subject)
                    .Where(t => t.Subject!.IsActive)
                    .Select(t => new { id = t.Subject!.Id, name = t.Subject.Name })
                    .Distinct()
                    .ToListAsync();
                
                return Json(subjects);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // AJAX endpoint to get topics by class and subject
        [HttpGet]
        public async Task<IActionResult> GetTopicsByClassAndSubject(int classId, int subjectId)
        {
            try
            {
                var topics = await _context.Topics
                    .Where(t => t.ClassId == classId && t.SubjectId == subjectId && t.IsActive)
                    .Select(t => new { id = t.Id, name = t.Name })
                    .ToListAsync();
                
                return Json(topics);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question, 
            IFormFile? questionImage,
            IFormFile? optionAImage,
            IFormFile? optionBImage,
            IFormFile? optionCImage,
            IFormFile? optionDImage,
            IFormFile? explanationImage)
        {
            if (ModelState.IsValid)
            {
                // Handle image uploads
                if (questionImage != null && questionImage.Length > 0)
                    question.QuestionImagePath = await SaveQuestionImageAsync(questionImage, "questions");

                if (optionAImage != null && optionAImage.Length > 0)
                    question.OptionAImagePath = await SaveQuestionImageAsync(optionAImage, "options");

                if (optionBImage != null && optionBImage.Length > 0)
                    question.OptionBImagePath = await SaveQuestionImageAsync(optionBImage, "options");

                if (optionCImage != null && optionCImage.Length > 0)
                    question.OptionCImagePath = await SaveQuestionImageAsync(optionCImage, "options");

                if (optionDImage != null && optionDImage.Length > 0)
                    question.OptionDImagePath = await SaveQuestionImageAsync(optionDImage, "options");

                if (explanationImage != null && explanationImage.Length > 0)
                    question.ExplanationImagePath = await SaveQuestionImageAsync(explanationImage, "explanations");

                // Process base64 images in question content
                question.QuestionText = await ProcessBase64ImagesAsync(question.QuestionText);
                question.OptionA = await ProcessBase64ImagesAsync(question.OptionA);
                question.OptionB = await ProcessBase64ImagesAsync(question.OptionB);
                question.OptionC = await ProcessBase64ImagesAsync(question.OptionC);
                question.OptionD = await ProcessBase64ImagesAsync(question.OptionD);
                question.Explanation = await ProcessBase64ImagesAsync(question.Explanation);

                // If question type is MCQWithAllOfAbove, set Option D to "All of the Above" and correct answer to D
                if (question.QuestionType == QuestionType.MCQWithAllOfAbove)
                {
                    question.OptionD = "All of the Above";
                    question.CorrectAnswer = "D";
                }

                question.CreatedDate = DateTime.UtcNow;
                question.CreatedBy = User.Identity?.Name;
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                
                // Get the topic to retrieve ClassId and SubjectId for maintaining selection
                var topic = await _context.Topics
                    .Include(t => t.Subject)
                    .Include(t => t.Class)
                    .FirstOrDefaultAsync(t => t.Id == question.TopicId);
                
                TempData["Success"] = "Question created successfully! You can add another question.";
                TempData["SelectedClassId"] = topic?.ClassId;
                TempData["SelectedSubjectId"] = topic?.SubjectId;
                TempData["SelectedTopicId"] = question.TopicId;
                
                return RedirectToAction(nameof(Create));
            }
            
            ViewData["ClassId"] = new SelectList(await _context.Classes.ToListAsync(), "Id", "Name");
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name");
            ViewData["TopicId"] = new SelectList(await _context.Topics.Include(t => t.Subject).Where(t => t.IsActive).ToListAsync(), "Id", "Name", question.TopicId);
            return View(question);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            ViewData["TopicId"] = new SelectList(await _context.Topics.Include(t => t.Subject).Where(t => t.IsActive).ToListAsync(), "Id", "Name", question.TopicId);
            ViewBag.TinyMceApiKey = await _tinyMceService.GetActiveApiKeyAsync();
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Question question,
            IFormFile? questionImage,
            IFormFile? optionAImage,
            IFormFile? optionBImage,
            IFormFile? optionCImage,
            IFormFile? optionDImage,
            IFormFile? explanationImage)
        {
            if (id != question.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingQuestion = await _context.Questions.AsNoTracking().FirstOrDefaultAsync(q => q.Id == id);
                    if (existingQuestion == null) return NotFound();

                    // Handle image uploads and deletions
                    if (questionImage != null && questionImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingQuestion.QuestionImagePath))
                            DeleteQuestionImage(existingQuestion.QuestionImagePath);
                        question.QuestionImagePath = await SaveQuestionImageAsync(questionImage, "questions");
                    }
                    else
                    {
                        question.QuestionImagePath = existingQuestion.QuestionImagePath;
                    }

                    if (optionAImage != null && optionAImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingQuestion.OptionAImagePath))
                            DeleteQuestionImage(existingQuestion.OptionAImagePath);
                        question.OptionAImagePath = await SaveQuestionImageAsync(optionAImage, "options");
                    }
                    else
                    {
                        question.OptionAImagePath = existingQuestion.OptionAImagePath;
                    }

                    if (optionBImage != null && optionBImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingQuestion.OptionBImagePath))
                            DeleteQuestionImage(existingQuestion.OptionBImagePath);
                        question.OptionBImagePath = await SaveQuestionImageAsync(optionBImage, "options");
                    }
                    else
                    {
                        question.OptionBImagePath = existingQuestion.OptionBImagePath;
                    }

                    if (optionCImage != null && optionCImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingQuestion.OptionCImagePath))
                            DeleteQuestionImage(existingQuestion.OptionCImagePath);
                        question.OptionCImagePath = await SaveQuestionImageAsync(optionCImage, "options");
                    }
                    else
                    {
                        question.OptionCImagePath = existingQuestion.OptionCImagePath;
                    }

                    if (optionDImage != null && optionDImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingQuestion.OptionDImagePath))
                            DeleteQuestionImage(existingQuestion.OptionDImagePath);
                        question.OptionDImagePath = await SaveQuestionImageAsync(optionDImage, "options");
                    }
                    else
                    {
                        question.OptionDImagePath = existingQuestion.OptionDImagePath;
                    }

                    if (explanationImage != null && explanationImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingQuestion.ExplanationImagePath))
                            DeleteQuestionImage(existingQuestion.ExplanationImagePath);
                        question.ExplanationImagePath = await SaveQuestionImageAsync(explanationImage, "explanations");
                    }
                    else
                    {
                        question.ExplanationImagePath = existingQuestion.ExplanationImagePath;
                    }

                    // Process base64 images in question content
                    question.QuestionText = await ProcessBase64ImagesAsync(question.QuestionText);
                    question.OptionA = await ProcessBase64ImagesAsync(question.OptionA);
                    question.OptionB = await ProcessBase64ImagesAsync(question.OptionB);
                    question.OptionC = await ProcessBase64ImagesAsync(question.OptionC);
                    question.OptionD = await ProcessBase64ImagesAsync(question.OptionD);
                    question.Explanation = await ProcessBase64ImagesAsync(question.Explanation);

                    // If question type is MCQWithAllOfAbove, set Option D to "All of the Above" and correct answer to D
                    if (question.QuestionType == QuestionType.MCQWithAllOfAbove)
                    {
                        question.OptionD = "All of the Above";
                        question.CorrectAnswer = "D";
                    }

                    _context.Update(question);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Question updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await QuestionExists(question.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TopicId"] = new SelectList(await _context.Topics.Include(t => t.Subject).Where(t => t.IsActive).ToListAsync(), "Id", "Name", question.TopicId);
            return View(question);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions
                .Include(q => q.Topic)
                .ThenInclude(t => t!.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null) return NotFound();

            return View(question);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions
                .Include(q => q.Topic)
                .ThenInclude(t => t!.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null) return NotFound();

            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                // Delete all associated images
                if (!string.IsNullOrEmpty(question.QuestionImagePath))
                    DeleteQuestionImage(question.QuestionImagePath);
                if (!string.IsNullOrEmpty(question.OptionAImagePath))
                    DeleteQuestionImage(question.OptionAImagePath);
                if (!string.IsNullOrEmpty(question.OptionBImagePath))
                    DeleteQuestionImage(question.OptionBImagePath);
                if (!string.IsNullOrEmpty(question.OptionCImagePath))
                    DeleteQuestionImage(question.OptionCImagePath);
                if (!string.IsNullOrEmpty(question.OptionDImagePath))
                    DeleteQuestionImage(question.OptionDImagePath);
                if (!string.IsNullOrEmpty(question.ExplanationImagePath))
                    DeleteQuestionImage(question.ExplanationImagePath);

                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Question deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> QuestionExists(int id)
        {
            return await _context.Questions.AnyAsync(e => e.Id == id);
        }

        // Helper method to save question images
        private async Task<string> SaveQuestionImageAsync(IFormFile image, string subfolder)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "questions", subfolder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/uploads/questions/{subfolder}/{uniqueFileName}";
        }

        // Helper method to process base64 images in content
        private async Task<string> ProcessBase64ImagesAsync(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            // Regular expression to find base64 images
            var base64Pattern = @"data:image/([^;]+);base64,([^""]+)";
            var matches = System.Text.RegularExpressions.Regex.Matches(content, base64Pattern);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                try
                {
                    var imageType = match.Groups[1].Value;
                    var base64Data = match.Groups[2].Value;
                    
                    // Convert base64 to bytes
                    var imageBytes = Convert.FromBase64String(base64Data);
                    
                    // Generate filename
                    var fileName = $"{Guid.NewGuid()}.{imageType}";
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "tinymce");
                    
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    
                    // Save the image file
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                    
                    // Replace the base64 URL with the file URL
                    var fileUrl = $"/uploads/tinymce/{fileName}";
                    content = content.Replace(match.Value, fileUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing base64 image");
                    // Keep the original base64 if conversion fails
                }
            }

            return content;
        }

        // Helper method to delete question images
        private void DeleteQuestionImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
    }
}

