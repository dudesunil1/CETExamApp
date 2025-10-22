using CETExamApp.Data;
using CETExamApp.Models;
using CETExamApp.Models.ViewModels;
using CETExamApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TestsController> _logger;

        public TestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<TestsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var tests = await _context.Tests
                .Include(t => t.Class)
                .Include(t => t.TestQuestions)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
            return View(tests);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            return View();
        }

        // Multi-Step Wizard Actions
        public async Task<IActionResult> CreateWizard()
        {
            var model = new TestCreationWizardViewModel
            {
                DurationMinutes = 180, // Default to 3 hours for CET
                TotalQuestions = 200, // Default total for CET (50+50+100)
                TotalMarks = 200,
                PassingMarks = 100,
                TestType = TestType.CET // Default to CET
            };

            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWizard(TestCreationWizardViewModel model)
        {
            try
            {
                _logger.LogInformation("CreateWizard called with model: Title={Title}, TestType={TestType}, ClassId={ClassId}", 
                    model.Title, model.TestType, model.ClassId);

                // Log model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid. Errors: {Errors}", 
                        string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }

                // For CET tests, enforce fixed values
                if (model.TestType == TestType.CET)
                {
                    model.DurationMinutes = 180; // Fixed 3 hours
                    
                    // Calculate total questions based on subject configs
                    if (model.SubjectConfigs != null && model.SubjectConfigs.Any())
                    {
                        model.TotalQuestions = model.SubjectConfigs.Sum(sc => sc.NumberOfQuestions);
                        model.TotalMarks = model.TotalQuestions; // Assuming 1 mark per question
                    }
                }

                // Validate CET/JEE subject combinations
                if (model.TestType == TestType.CET || model.TestType == TestType.JEE)
                {
                    var validationResult = ValidateCETJEESubjectCombination(model.SubjectConfigs);
                    if (!validationResult.IsValid)
                    {
                        ModelState.AddModelError("SubjectConfigs", validationResult.ErrorMessage);
                    }
                }

                if (ModelState.IsValid)
                {
                    _logger.LogInformation("ModelState is valid, creating test...");
                    
                    // No need to set SubjectId or GroupId as they are removed from the model
                    
                    var test = new Test
                    {
                        Title = model.Title,
                        Description = model.Description,
                        DurationMinutes = model.DurationMinutes,
                        TotalMarks = model.TotalMarks,
                        PassingMarks = model.PassingMarks,
                        ClassId = model.ClassId,
                        TestType = model.TestType,
                        ShuffleQuestions = model.ShuffleQuestions,
                        ShowResultsImmediately = true,
                        AllowLateSubmission = false,
                        Status = model.Status,
                        CreatedDate = DateTimeExtensions.NowIST(),
                        CreatedBy = User.Identity?.Name
                    };

                    _context.Tests.Add(test);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Test created successfully with ID: {TestId}", test.Id);

                    // Save subject configurations and questions
                    await SaveTestQuestions(test.Id, model.SelectedQuestionsBySubject, model.SubjectConfigs);

                    TempData["Success"] = "Test created successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogWarning("ModelState validation failed. Returning to view with errors.");
                }

                ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateWizard action");
                TempData["Error"] = "An error occurred while creating the test: " + ex.Message;
                ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
                return View(model);
            }
        }

        private async Task SaveTestQuestions(int testId, Dictionary<int, List<int>> selectedQuestions, List<SubjectConfigViewModel> subjectConfigs)
        {
            var order = 1;
            foreach (var kvp in selectedQuestions)
            {
                var subjectId = kvp.Key;
                var questionIds = kvp.Value;

                // SubjectId is already set correctly in the test, no need to update it here

                foreach (var questionId in questionIds)
                {
                    var testQuestion = new TestQuestion
                    {
                        TestId = testId,
                        QuestionId = questionId,
                        QuestionOrder = order++,
                        Marks = 1 // Default marks, can be customized later
                    };

                    _context.TestQuestions.Add(testQuestion);
                }
            }

            await _context.SaveChangesAsync();
        }

        private ValidationResult ValidateCETJEESubjectCombination(List<SubjectConfigViewModel> subjectConfigs)
        {
            // Get subjects with questions > 0
            var selectedSubjects = subjectConfigs
                .Where(sc => sc.NumberOfQuestions > 0)
                .Select(sc => sc.SubjectName.ToLower().Trim())
                .ToList();

            if (selectedSubjects.Count == 0)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = "At least one subject must be selected for CET/JEE tests." };
            }

            // Check for Physics and Chemistry (required for both combinations)
            var hasPhysics = selectedSubjects.Any(s => s.Contains("physics"));
            var hasChemistry = selectedSubjects.Any(s => s.Contains("chemistry"));
            var hasMaths = selectedSubjects.Any(s => s.Contains("math") || s.Contains("mathematics"));
            var hasBiology = selectedSubjects.Any(s => s.Contains("biology"));

            if (!hasPhysics || !hasChemistry)
            {
                return new ValidationResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "CET/JEE tests must include both Physics and Chemistry subjects." 
                };
            }

            // Check for valid combinations
            if (hasMaths && hasBiology)
            {
                return new ValidationResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "CET/JEE tests cannot include both Mathematics and Biology. Choose either Physics + Chemistry + Mathematics OR Physics + Chemistry + Biology." 
                };
            }

            if (!hasMaths && !hasBiology)
            {
                return new ValidationResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "CET/JEE tests must include either Mathematics or Biology along with Physics and Chemistry." 
                };
            }

            return new ValidationResult { IsValid = true };
        }

        [HttpPost]
        public IActionResult ValidateSubjectCombination(int testType, List<SubjectConfigViewModel> subjectConfigs)
        {
            if (testType == (int)TestType.CET || testType == (int)TestType.JEE)
            {
                var validationResult = ValidateCETJEESubjectCombination(subjectConfigs);
                return Json(validationResult);
            }
            
            return Json(new ValidationResult { IsValid = true });
        }

        [HttpGet]
        public IActionResult GetCETQuestionCounts(string groupType)
        {
            var questionCounts = new Dictionary<string, int>();
            
            if (groupType?.ToUpper() == "PCM")
            {
                questionCounts = new Dictionary<string, int>
                {
                    { "Physics", 50 },
                    { "Chemistry", 50 },
                    { "Mathematics", 50 }
                };
            }
            else if (groupType?.ToUpper() == "PCB")
            {
                questionCounts = new Dictionary<string, int>
                {
                    { "Physics", 50 },
                    { "Chemistry", 50 },
                    { "Biology", 100 }
                };
            }
            
            return Json(new
            {
                questionCounts = questionCounts,
                totalQuestions = questionCounts.Values.Sum(),
                duration = 180 // Fixed 3 hours for CET
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Test test)
        {
            if (ModelState.IsValid)
            {
                test.CreatedDate = DateTime.UtcNow;
                test.CreatedBy = User.Identity?.Name;
                
                _context.Tests.Add(test);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Test created successfully! Now add questions to the test.";
                return RedirectToAction(nameof(AddQuestions), new { id = test.Id });
            }
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", test.ClassId);
            
            return View(test);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var test = await _context.Tests.FindAsync(id);
            if (test == null) return NotFound();

            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", test.ClassId);
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
            ViewData["ClassId"] = new SelectList(await _context.Classes.Where(c => c.IsActive).ToListAsync(), "Id", "Name", test.ClassId);
            return View(test);
        }

        public async Task<IActionResult> AddQuestions(int? id, int? topicId)
        {
            if (id == null) return NotFound();

            var test = await _context.Tests
                .Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                    .ThenInclude(q => q!.Topic)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null) return NotFound();

            // Get all topics for the test's class
            var topics = await _context.Topics
                .Where(t => t.ClassId == test.ClassId && t.IsActive)
                .ToListAsync();

            // Get available questions - filter by topic if selected
            var availableQuestionsQuery = _context.Questions
                .Include(q => q.Topic)
                .Where(q => q.Topic!.ClassId == test.ClassId && q.IsActive);

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

        // AJAX Endpoints for Multi-Step Wizard
        [HttpGet]
        public async Task<IActionResult> GetSubjectsForClass(int classId)
        {
            try
            {
                var subjects = await _context.Topics
                    .Where(t => t.ClassId == classId && t.IsActive)
                    .Include(t => t.Subject)
                    .Select(t => t.Subject)
                    .Distinct()
                    .Where(s => s.IsActive)
                    .Select(s => new { id = s.Id, name = s.Name })
                    .ToListAsync();

                return Json(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subjects for class");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTopicsForSubject(int subjectId, int classId)
        {
            try
            {
                var topics = await _context.Topics
                    .Where(t => t.SubjectId == subjectId && t.ClassId == classId && t.IsActive)
                    .Select(t => new { id = t.Id, name = t.Name })
                    .ToListAsync();

                return Json(topics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting topics for subject");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestionsForTopics(string topicIds)
        {
            try
            {
                var topicIdList = topicIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

                var questions = await _context.Questions
                    .Include(q => q.Topic)
                    .Where(q => topicIdList.Contains(q.TopicId) && q.IsActive)
                    .Select(q => new QuestionSelectionViewModel
                    {
                        QuestionId = q.Id,
                        QuestionTextPreview = q.QuestionText ?? "",
                        QuestionType = q.QuestionType.ToString(),
                        DifficultyLevel = q.DifficultyLevel.ToString(),
                        Marks = q.Marks,
                        TopicId = q.TopicId,
                        TopicName = q.Topic!.Name
                    })
                    .ToListAsync();

                // Strip HTML tags from question text for display
                foreach (var question in questions)
                {
                    question.QuestionTextPreview = System.Text.RegularExpressions.Regex.Replace(
                        question.QuestionTextPreview, "<.*?>", string.Empty);
                    if (question.QuestionTextPreview.Length > 100)
                    {
                        question.QuestionTextPreview = question.QuestionTextPreview.Substring(0, 100) + "...";
                    }
                }

                return Json(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting questions for topics");
                return Json(new List<object>());
            }
        }

        // AJAX Endpoints for Enhanced Functionality

        [HttpGet]
        public async Task<IActionResult> GetGroupsByClass(int classId)
        {
            try
            {
                var groups = await _context.Groups
                    .Where(g => g.ClassId == classId && g.IsActive)
                    .Select(g => new { id = g.Id, name = g.Name })
                    .ToListAsync();

                return Json(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting groups by class");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectsByGroup(int classId, int groupId)
        {
            try
            {
                // Get the group to determine which subjects it includes
                var group = await _context.Groups.FindAsync(groupId);
                if (group == null) return Json(new List<object>());

                var subjects = new List<object>();

                // Map group names to subjects
                if (group.Name.Contains("Physics"))
                    subjects.Add(new { id = GetSubjectIdByName("Physics"), name = "Physics" });
                if (group.Name.Contains("Chemistry"))
                    subjects.Add(new { id = GetSubjectIdByName("Chemistry"), name = "Chemistry" });
                if (group.Name.Contains("Math"))
                    subjects.Add(new { id = GetSubjectIdByName("Maths"), name = "Maths" });
                if (group.Name.Contains("Biology"))
                    subjects.Add(new { id = GetSubjectIdByName("Biology"), name = "Biology" });

                return Json(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subjects by group");
                return Json(new List<object>());
            }
        }

        private int GetSubjectIdByName(string subjectName)
        {
            // This is a helper method to get subject ID by name
            // In a real scenario, you might want to cache this or make it async
            var subject = _context.Subjects.FirstOrDefault(s => s.Name == subjectName);
            return subject?.Id ?? 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectsByClass(int classId)
        {
            try
            {
                var subjects = await _context.Topics
                    .Where(t => t.ClassId == classId && t.IsActive)
                    .Include(t => t.Subject)
                    .Select(t => t.Subject)
                    .Distinct()
                    .Where(s => s.IsActive)
                    .Select(s => new { id = s.Id, name = s.Name })
                    .ToListAsync();

                return Json(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subjects by class");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTopicsBySubject(int subjectId, int classId)
        {
            try
            {
                var topics = await _context.Topics
                    .Where(t => t.SubjectId == subjectId && t.ClassId == classId && t.IsActive)
                    .Select(t => new { id = t.Id, name = t.Name })
                    .ToListAsync();

                return Json(topics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting topics by subject");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestionsByTopics(string topicIds, int? subjectId = null, int requiredCount = 0)
        {
            try
            {
                var topicIdList = topicIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

                var questions = await _context.Questions
                    .Include(q => q.Topic)
                    .Where(q => topicIdList.Contains(q.TopicId) && q.IsActive)
                    .Select(q => new QuestionSelectionViewModel
                    {
                        QuestionId = q.Id,
                        QuestionTextPreview = q.QuestionText ?? "",
                        QuestionType = q.QuestionType.ToString(),
                        DifficultyLevel = q.DifficultyLevel.ToString(),
                        Marks = q.Marks,
                        TopicId = q.TopicId,
                        TopicName = q.Topic!.Name
                    })
                    .ToListAsync();

                // Strip HTML tags from question text for display
                foreach (var question in questions)
                {
                    question.QuestionTextPreview = System.Text.RegularExpressions.Regex.Replace(
                        question.QuestionTextPreview, "<.*?>", string.Empty);
                    if (question.QuestionTextPreview.Length > 100)
                    {
                        question.QuestionTextPreview = question.QuestionTextPreview.Substring(0, 100) + "...";
                    }
                }

                return Json(new
                {
                    questions = questions,
                    totalCount = questions.Count,
                    requiredCount = requiredCount,
                    canProceed = requiredCount == 0 || questions.Count >= requiredCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting questions by topics");
                return Json(new { questions = new List<object>(), totalCount = 0, requiredCount = 0, canProceed = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsByGroup(int groupId)
        {
            try
            {
                var students = await _userManager.GetUsersInRoleAsync("Student");
                var studentsInGroup = students
                    .Where(s => s.GroupId == groupId)
                    .Select(s => new StudentSelectionViewModel
                    {
                        StudentId = s.Id,
                        StudentName = $"{s.FirstName} {s.LastName}".Trim(),
                        IsSelected = false
                    })
                    .ToList();

                return Json(studentsInGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting students by group");
                return Json(new List<object>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveMultiSubjectQuestionSelection(int testId, string subjectQuestionsData)
        {
            try
            {
                var subjectQuestions = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<int>>>(subjectQuestionsData);
                
                // Clear existing questions for this test
                var existingQuestions = await _context.TestQuestions
                    .Where(tq => tq.TestId == testId)
                    .ToListAsync();
                
                _context.TestQuestions.RemoveRange(existingQuestions);
                
                // Add new question selections
                var order = 1;
                foreach (var kvp in subjectQuestions)
                {
                    var questionIds = kvp.Value;
                    
                    foreach (var questionId in questionIds)
                    {
                        var testQuestion = new TestQuestion
                        {
                            TestId = testId,
                            QuestionId = questionId,
                            QuestionOrder = order++,
                            Marks = 1 // Default marks, can be customized later
                        };
                        
                        _context.TestQuestions.Add(testQuestion);
                    }
                }
                
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, redirectUrl = Url.Action("StudentAllocation", new { id = testId }) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving multi-subject question selection");
                return Json(new { success = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> StudentAllocation(int? id)
        {
            if (id == null) return NotFound();

            var test = await _context.Tests
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null) return NotFound();

            ViewBag.Test = test;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AllocateStudents(int testId, List<string> selectedStudentIds)
        {
            try
            {
                // Clear existing allocations for this test
                var existingAllocations = await _context.TestAllocations
                    .Where(ta => ta.TestId == testId)
                    .ToListAsync();
                
                _context.TestAllocations.RemoveRange(existingAllocations);
                
                // Add new allocations
                foreach (var studentId in selectedStudentIds)
                {
                    var allocation = new TestAllocation
                    {
                        TestId = testId,
                        StudentId = studentId,
                        ScheduledStartTime = DateTimeExtensions.NowIST().AddHours(1), // Default to 1 hour ahead in IST
                        ScheduledEndTime = DateTimeExtensions.NowIST().AddHours(2), // Default to 2 hours ahead in IST
                        AllocatedBy = User.Identity?.Name,
                        AllocatedDate = DateTimeExtensions.NowIST()
                    };
                    
                    _context.TestAllocations.Add(allocation);
                }
                
                await _context.SaveChangesAsync();
                
                TempData["Success"] = $"Test allocated to {selectedStudentIds.Count} students successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating students to test");
                TempData["Error"] = "Error allocating students to test: " + ex.Message;
                return RedirectToAction("StudentAllocation", new { id = testId });
            }
        }
    }
}

