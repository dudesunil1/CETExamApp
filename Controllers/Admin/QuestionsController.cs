using CETExamApp.Data;
using CETExamApp.Models;
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

        public QuestionsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int? topicId, int? difficultyLevel)
        {
            var questionsQuery = _context.Questions
                .Include(q => q.Topic)
                .ThenInclude(t => t!.Subject)
                .AsQueryable();

            if (topicId.HasValue)
                questionsQuery = questionsQuery.Where(q => q.TopicId == topicId.Value);

            if (difficultyLevel.HasValue)
                questionsQuery = questionsQuery.Where(q => (int)q.DifficultyLevel == difficultyLevel.Value);

            var questions = await questionsQuery
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();

            ViewData["TopicId"] = new SelectList(await _context.Topics.Include(t => t.Subject).ToListAsync(), "Id", "Name", topicId);
            ViewData["DifficultyLevel"] = new SelectList(Enum.GetValues(typeof(DifficultyLevel)), difficultyLevel);

            return View(questions);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["TopicId"] = new SelectList(await _context.Topics.Include(t => t.Subject).Where(t => t.IsActive).ToListAsync(), "Id", "Name");
            return View();
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

                question.CreatedDate = DateTime.UtcNow;
                question.CreatedBy = User.Identity?.Name;
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Question created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["TopicId"] = new SelectList(await _context.Topics.Include(t => t.Subject).Where(t => t.IsActive).ToListAsync(), "Id", "Name", question.TopicId);
            return View(question);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            ViewData["TopicId"] = new SelectList(await _context.Topics.Include(t => t.Subject).Where(t => t.IsActive).ToListAsync(), "Id", "Name", question.TopicId);
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

