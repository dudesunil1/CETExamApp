using CETExamApp.Data;
using CETExamApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tests = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .Include(t => t.Group)
                .Include(t => t.TestQuestions)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
            return View(tests);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name");
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Test test)
        {
            if (ModelState.IsValid)
            {
                test.CreatedDate = DateTime.UtcNow;
                test.CreatedBy = User.Identity?.Name;
                
                // Convert local time to UTC for storage
                if (test.StartDateTime.HasValue)
                    test.StartDateTime = test.StartDateTime.Value.ToUniversalTime();
                if (test.EndDateTime.HasValue)
                    test.EndDateTime = test.EndDateTime.Value.ToUniversalTime();
                
                _context.Tests.Add(test);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Test created successfully! Now add questions to the test.";
                return RedirectToAction(nameof(AddQuestions), new { id = test.Id });
            }
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name", test.SubjectId);
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", test.ClassId);
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name", test.GroupId);
            
            // Get topics for the selected subject
            var topics = await _context.Topics
                .Where(t => t.SubjectId == test.SubjectId && t.IsActive)
                .ToListAsync();
            ViewData["Topics"] = new SelectList(topics, "Id", "Name");
            
            return View(test);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var test = await _context.Tests.FindAsync(id);
            if (test == null) return NotFound();

            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name", test.SubjectId);
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", test.ClassId);
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name", test.GroupId);
            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Test test)
        {
            if (id != test.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Convert local time to UTC for storage
                    if (test.StartDateTime.HasValue)
                        test.StartDateTime = test.StartDateTime.Value.ToUniversalTime();
                    if (test.EndDateTime.HasValue)
                        test.EndDateTime = test.EndDateTime.Value.ToUniversalTime();
                    
                    _context.Update(test);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Test updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TestExists(test.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(await _context.Subjects.Where(s => s.IsActive).ToListAsync(), "Id", "Name", test.SubjectId);
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", test.ClassId);
            ViewData["GroupId"] = new SelectList(await _context.Groups.Where(g => g.IsActive).Include(g => g.Class).ToListAsync(), "Id", "Name", test.GroupId);
            return View(test);
        }

        public async Task<IActionResult> AddQuestions(int? id, int? topicId)
        {
            if (id == null) return NotFound();

            var test = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.Group)
                .Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                    .ThenInclude(q => q!.Topic)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null) return NotFound();

            // Get topics for the test's subject
            var topics = await _context.Topics
                .Where(t => t.SubjectId == test.SubjectId && t.IsActive)
                .ToListAsync();

            // Get available questions - filter by topic if selected
            var availableQuestionsQuery = _context.Questions
                .Include(q => q.Topic)
                .Where(q => q.Topic!.SubjectId == test.SubjectId && q.IsActive);

            if (topicId.HasValue)
                availableQuestionsQuery = availableQuestionsQuery.Where(q => q.TopicId == topicId.Value);

            var availableQuestions = await availableQuestionsQuery.ToListAsync();

            ViewBag.Test = test;
            ViewBag.Topics = new SelectList(topics, "Id", "Name", topicId);
            ViewBag.AvailableQuestions = availableQuestions;
            ViewBag.SelectedTopicId = topicId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestionToTest(int testId, int questionId, int marks)
        {
            var test = await _context.Tests.Include(t => t.TestQuestions).FirstOrDefaultAsync(t => t.Id == testId);
            if (test == null) return NotFound();

            // Check if question already added
            if (test.TestQuestions!.Any(tq => tq.QuestionId == questionId))
            {
                TempData["Error"] = "Question already added to this test!";
                return RedirectToAction(nameof(AddQuestions), new { id = testId });
            }

            var testQuestion = new TestQuestion
            {
                TestId = testId,
                QuestionId = questionId,
                QuestionOrder = test.TestQuestions.Count + 1,
                Marks = marks
            };

            _context.TestQuestions.Add(testQuestion);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Question added to test successfully!";
            return RedirectToAction(nameof(AddQuestions), new { id = testId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveQuestionFromTest(int testId, int questionId)
        {
            var testQuestion = await _context.TestQuestions
                .FirstOrDefaultAsync(tq => tq.TestId == testId && tq.QuestionId == questionId);

            if (testQuestion != null)
            {
                _context.TestQuestions.Remove(testQuestion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Question removed from test!";
            }

            return RedirectToAction(nameof(AddQuestions), new { id = testId });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var test = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (test == null) return NotFound();

            return View(test);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var test = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (test == null) return NotFound();

            return View(test);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test != null)
            {
                _context.Tests.Remove(test);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Test deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TestExists(int id)
        {
            return await _context.Tests.AnyAsync(e => e.Id == id);
        }
    }
}

