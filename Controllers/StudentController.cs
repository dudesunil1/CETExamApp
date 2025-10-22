using CETExamApp.Data;
using CETExamApp.Models;
using CETExamApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<StudentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> TestInstructions(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Check if test is allocated to this student and starts within 30 minutes
            var testAllocation = await _context.TestAllocations
                .Include(ta => ta.Test)
                    .ThenInclude(t => t!.TestQuestions)
                .FirstOrDefaultAsync(ta => ta.TestId == id && ta.StudentId == user.Id);

            if (testAllocation == null)
            {
                TempData["Error"] = "Test not found or not allocated to you.";
                return RedirectToAction("Dashboard");
            }

            var test = testAllocation.Test;
            if (test == null)
            {
                TempData["Error"] = "Test not found.";
                return RedirectToAction("Dashboard");
            }

            // Check if test starts within 30 minutes
            var timeUntilStart = testAllocation.ScheduledStartTime - DateTime.UtcNow;
            if (timeUntilStart?.TotalMinutes > 30 || timeUntilStart?.TotalMinutes < 0)
            {
                TempData["Error"] = "Test instructions are only available 30 minutes before the scheduled start time.";
                return RedirectToAction("Dashboard");
            }

            // Check if student has already started this test
            var existingAttempt = await _context.TestAttempts
                .FirstOrDefaultAsync(ta => ta.TestId == id && ta.StudentId == user.Id);

            if (existingAttempt != null && existingAttempt.Status != TestAttemptStatus.NotStarted)
            {
                TempData["Error"] = "You have already started this test.";
                return RedirectToAction("Dashboard");
            }

            return View(test);
        }

        public async Task<IActionResult> CETExam(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Check if test is allocated to this student
            var testAllocation = await _context.TestAllocations
                .Include(ta => ta.Test)
                    .ThenInclude(t => t!.TestQuestions)
                        .ThenInclude(tq => tq.Question)
                .FirstOrDefaultAsync(ta => ta.TestId == id && ta.StudentId == user.Id);

            if (testAllocation == null)
            {
                TempData["Error"] = "Test not found or not allocated to you.";
                return RedirectToAction("Dashboard");
            }

            var test = testAllocation.Test;
            if (test == null)
            {
                TempData["Error"] = "Test not found.";
                return RedirectToAction("Dashboard");
            }

            // Check if it's a CET test
            if (test.TestType != TestType.CET)
            {
                TempData["Error"] = "This is not a CET test.";
                return RedirectToAction("Dashboard");
            }

            // Check if test has started
            var timeUntilStart = testAllocation.ScheduledStartTime - DateTime.UtcNow;
            if (timeUntilStart?.TotalMinutes > 0)
            {
                TempData["Error"] = "Test has not started yet.";
                return RedirectToAction("Dashboard");
            }

            // Check if test has expired
            var timeUntilEnd = testAllocation.ScheduledEndTime - DateTime.UtcNow;
            if (timeUntilEnd?.TotalMinutes < 0 && !test.AllowLateSubmission)
            {
                TempData["Error"] = "Test has expired.";
                return RedirectToAction("Dashboard");
            }

            return View(test);
        }

        public async Task<IActionResult> CETResults(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Get the test
            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                TempData["Error"] = "Test not found.";
                return RedirectToAction("Dashboard");
            }

            return View(test);
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Get upcoming tests (allocated but not attempted or in progress)
            var upcomingTests = await _context.TestAllocations
                .Include(ta => ta.Test)
                    .ThenInclude(t => t!.TestQuestions)
                .Where(ta => ta.StudentId == user.Id && !ta.IsCompleted)
                .OrderBy(ta => ta.ScheduledStartTime)
                .ToListAsync();

            // Get completed tests (with results)
            var completedTests = await _context.TestResults
                .Include(tr => tr.Test)
                .Where(tr => tr.StudentId == user.Id)
                .OrderByDescending(tr => tr.SubmittedAt)
                .ToListAsync();

            // Get in-progress tests
            var inProgressTests = await _context.TestAttempts
                .Include(ta => ta.Test)
                .Where(ta => ta.StudentId == user.Id && ta.Status == TestAttemptStatus.InProgress)
                .ToListAsync();

            // Create a mapping of test results to attempts
            var resultToAttemptMap = new Dictionary<int, int>();
            foreach (var result in completedTests)
            {
                var testAttempt = await _context.TestAttempts
                    .Where(ta => ta.TestId == result.TestId && ta.StudentId == user.Id)
                    .OrderByDescending(ta => ta.StartedAt)
                    .FirstOrDefaultAsync();
                
                if (testAttempt != null)
                {
                    resultToAttemptMap[result.Id] = testAttempt.Id;
                }
            }

            ViewBag.UpcomingTests = upcomingTests;
            ViewBag.CompletedTests = completedTests;
            ViewBag.InProgressTests = inProgressTests;
            ViewBag.ResultToAttemptMap = resultToAttemptMap;

            return View(user);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        // Start Test
        public async Task<IActionResult> StartTest(int allocationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var allocation = await _context.TestAllocations
                .Include(ta => ta.Test)
                    .ThenInclude(t => t!.TestQuestions)
                        .ThenInclude(tq => tq.Question)
                .FirstOrDefaultAsync(ta => ta.Id == allocationId && ta.StudentId == user.Id);

            if (allocation == null)
            {
                TempData["Error"] = "Test allocation not found!";
                return RedirectToAction("Dashboard");
            }

            // Check if test can be started (time validation)
            if (allocation.ScheduledStartTime.HasValue && DateTime.UtcNow < allocation.ScheduledStartTime.Value)
            {
                TempData["Error"] = "Test has not started yet. Please wait until the scheduled time.";
                return RedirectToAction("Dashboard");
            }

            // Check if already attempted
            var existingAttempt = await _context.TestAttempts
                .FirstOrDefaultAsync(ta => ta.TestAllocationId == allocationId && ta.StudentId == user.Id);

            if (existingAttempt != null && existingAttempt.Status == TestAttemptStatus.Submitted)
            {
                TempData["Error"] = "You have already completed this test!";
                return RedirectToAction("Dashboard");
            }

            // Check if test time has expired
            if (allocation.ScheduledEndTime.HasValue && DateTime.UtcNow > allocation.ScheduledEndTime.Value && !allocation.Test!.AllowLateSubmission)
            {
                TempData["Error"] = "Test time has expired!";
                return RedirectToAction("Dashboard");
            }

            // Create or resume test attempt
            TestAttempt attempt;
            if (existingAttempt == null)
            {
                // Shuffle questions if enabled
                string? shuffledOrder = null;
                if (allocation.Test!.ShuffleQuestions)
                {
                    var questionIds = allocation.Test.TestQuestions!
                        .OrderBy(tq => tq.QuestionOrder)
                        .Select(tq => tq.Id)
                        .ToList();
                    
                    // Fisher-Yates shuffle
                    var rng = new Random();
                    int n = questionIds.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        (questionIds[k], questionIds[n]) = (questionIds[n], questionIds[k]);
                    }
                    
                    shuffledOrder = string.Join(",", questionIds);
                }

                attempt = new TestAttempt
                {
                    TestAllocationId = allocationId,
                    StudentId = user.Id,
                    TestId = allocation.Test!.Id,
                    StartedAt = DateTimeExtensions.NowIST(),
                    Status = TestAttemptStatus.InProgress,
                    ShuffledQuestionOrder = shuffledOrder
                };

                _context.TestAttempts.Add(attempt);
                await _context.SaveChangesAsync();
            }
            else
            {
                attempt = existingAttempt;
            }

            return RedirectToAction("TakeTest", new { attemptId = attempt.Id });
        }

        // Take Test Interface
        public async Task<IActionResult> TakeTest(int attemptId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var attempt = await _context.TestAttempts
                .Include(ta => ta.Test)
                    .ThenInclude(t => t!.TestQuestions)
                        .ThenInclude(tq => tq.Question)
                            .ThenInclude(q => q!.Topic)
                .Include(ta => ta.TestAllocation)
                .FirstOrDefaultAsync(ta => ta.Id == attemptId && ta.StudentId == user.Id);

            if (attempt == null)
            {
                TempData["Error"] = "Test attempt not found!";
                return RedirectToAction("Dashboard");
            }

            if (attempt.Status == TestAttemptStatus.Submitted)
            {
                return RedirectToAction("TestReview", new { attemptId });
            }

            // Check time expiry
            var timeElapsed = (DateTime.UtcNow - attempt.StartedAt).TotalMinutes;
            if (timeElapsed > attempt.Test!.DurationMinutes)
            {
                // Auto-submit
                await SubmitTestAuto(attemptId);
                return RedirectToAction("TestReview", new { attemptId });
            }

            // Get or create test result
            var testResult = await _context.TestResults
                .Include(tr => tr.StudentAnswers)
                .FirstOrDefaultAsync(tr => tr.TestId == attempt.TestId && tr.StudentId == user.Id);

            if (testResult == null)
            {
                testResult = new TestResult
                {
                    TestId = attempt.TestId,
                    StudentId = user.Id,
                    StartedAt = attempt.StartedAt,
                    TotalMarks = attempt.Test.TotalMarks,
                    ObtainedMarks = 0,
                    Percentage = 0,
                    IsPassed = false
                };
                _context.TestResults.Add(testResult);
                await _context.SaveChangesAsync();
            }

            // Get questions in shuffled order if applicable
            List<TestQuestion> questions;
            if (!string.IsNullOrEmpty(attempt.ShuffledQuestionOrder))
            {
                var questionIds = attempt.ShuffledQuestionOrder.Split(',').Select(int.Parse).ToList();
                questions = new List<TestQuestion>();
                foreach (var id in questionIds)
                {
                    var tq = attempt.Test.TestQuestions!.FirstOrDefault(q => q.Id == id);
                    if (tq != null) questions.Add(tq);
                }
            }
            else
            {
                questions = attempt.Test.TestQuestions!.OrderBy(tq => tq.QuestionOrder).ToList();
            }

            // Initialize student answers if not exists
            foreach (var tq in questions)
            {
                if (!testResult.StudentAnswers!.Any(sa => sa.QuestionId == tq.QuestionId))
                {
                    var answer = new StudentAnswer
                    {
                        TestResultId = testResult.Id,
                        QuestionId = tq.QuestionId,
                        Status = QuestionStatus.Unvisited
                    };
                    _context.StudentAnswers.Add(answer);
                }
            }
            await _context.SaveChangesAsync();

            ViewBag.TestAttempt = attempt;
            ViewBag.TestResult = testResult;
            ViewBag.Questions = questions;
            ViewBag.TimeRemaining = attempt.Test.DurationMinutes - (int)timeElapsed;

            return View();
        }

        // Save Answer (AJAX)
        [HttpPost]
        public async Task<IActionResult> SaveAnswer(int attemptId, int questionId, string? answer, bool markForReview)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var attempt = await _context.TestAttempts
                .Include(ta => ta.Test)
                .FirstOrDefaultAsync(ta => ta.Id == attemptId && ta.StudentId == user.Id);

            if (attempt == null) return NotFound();

            var testResult = await _context.TestResults
                .Include(tr => tr.StudentAnswers)
                .FirstOrDefaultAsync(tr => tr.TestId == attempt.TestId && tr.StudentId == user.Id);

            if (testResult == null) return NotFound();

            var studentAnswer = testResult.StudentAnswers!.FirstOrDefault(sa => sa.QuestionId == questionId);
            if (studentAnswer == null)
            {
                studentAnswer = new StudentAnswer
                {
                    TestResultId = testResult.Id,
                    QuestionId = questionId
                };
                _context.StudentAnswers.Add(studentAnswer);
            }

            studentAnswer.AnswerText = answer;
            studentAnswer.AnsweredAt = DateTimeExtensions.NowIST();
            studentAnswer.IsMarkedForReview = markForReview;

            // Update status
            if (!string.IsNullOrEmpty(answer))
            {
                studentAnswer.Status = markForReview ? QuestionStatus.MarkedForReview : QuestionStatus.Answered;
            }
            else
            {
                studentAnswer.Status = QuestionStatus.Visited;
            }

            attempt.LastActivityAt = DateTimeExtensions.NowIST();
            await _context.SaveChangesAsync();

            return Json(new { success = true, status = studentAnswer.Status.ToString() });
        }

        // Mark Question as Visited (AJAX)
        [HttpPost]
        public async Task<IActionResult> MarkVisited(int attemptId, int questionId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // First, get the TestId from the attempt
            var testId = await _context.TestAttempts
                .Where(ta => ta.Id == attemptId)
                .Select(ta => ta.TestId)
                .FirstOrDefaultAsync();

            // Then, get the test result
            var testResult = await _context.TestResults
                .Include(tr => tr.StudentAnswers)
                .FirstOrDefaultAsync(tr => tr.TestId == testId && tr.StudentId == user.Id);

            if (testResult == null) return NotFound();

            var studentAnswer = testResult.StudentAnswers!.FirstOrDefault(sa => sa.QuestionId == questionId);
            if (studentAnswer != null && studentAnswer.Status == QuestionStatus.Unvisited)
            {
                studentAnswer.Status = QuestionStatus.Visited;
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // Submit Test
        [HttpPost]
        public async Task<IActionResult> SubmitTest(int attemptId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await SubmitTestAuto(attemptId);

            TempData["Success"] = "Test submitted successfully!";
            return RedirectToAction("TestReview", new { attemptId });
        }

        // Helper method to submit test
        private async Task SubmitTestAuto(int attemptId)
        {
            var attempt = await _context.TestAttempts
                .Include(ta => ta.Test)
                .FirstOrDefaultAsync(ta => ta.Id == attemptId);

            if (attempt == null) return;

            attempt.SubmittedAt = DateTimeExtensions.NowIST();
            attempt.Status = TestAttemptStatus.Submitted;

            var allocation = await _context.TestAllocations.FindAsync(attempt.TestAllocationId);
            if (allocation != null)
            {
                allocation.IsCompleted = true;
                allocation.CompletedDate = DateTimeExtensions.NowIST();
            }

            var testResult = await _context.TestResults
                .Include(tr => tr.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                .FirstOrDefaultAsync(tr => tr.TestId == attempt.TestId && tr.StudentId == attempt.StudentId);

            if (testResult != null)
            {
                // Calculate results
                testResult.SubmittedAt = DateTimeExtensions.NowIST();
                testResult.TimeTakenMinutes = (int)(DateTimeExtensions.NowIST() - attempt.StartedAt).TotalMinutes;

                int obtainedMarks = 0;
                foreach (var answer in testResult.StudentAnswers!)
                {
                    if (answer.Question != null && !string.IsNullOrEmpty(answer.AnswerText))
                    {
                        answer.IsCorrect = answer.AnswerText.Trim().Equals(answer.Question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
                        
                        var testQuestion = await _context.TestQuestions
                            .FirstOrDefaultAsync(tq => tq.TestId == testResult.TestId && tq.QuestionId == answer.QuestionId);
                        
                        if (answer.IsCorrect && testQuestion != null)
                        {
                            answer.MarksObtained = testQuestion.Marks;
                            obtainedMarks += testQuestion.Marks;
                        }
                        else
                        {
                            answer.MarksObtained = 0;
                        }
                    }
                }

                testResult.ObtainedMarks = obtainedMarks;
                testResult.Percentage = testResult.TotalMarks > 0 
                    ? (decimal)obtainedMarks / testResult.TotalMarks * 100 
                    : 0;
                testResult.IsPassed = obtainedMarks >= attempt.Test!.PassingMarks;
            }

            await _context.SaveChangesAsync();
        }

        // Test Review (after submission)
        public async Task<IActionResult> TestReview(int attemptId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var attempt = await _context.TestAttempts
                .Include(ta => ta.Test)
                .FirstOrDefaultAsync(ta => ta.Id == attemptId && ta.StudentId == user.Id);

            if (attempt == null)
            {
                TempData["Error"] = "Test attempt not found!";
                return RedirectToAction("Dashboard");
            }

            var testResult = await _context.TestResults
                .Include(tr => tr.Test)
                .Include(tr => tr.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                        .ThenInclude(q => q!.Topic)
                .FirstOrDefaultAsync(tr => tr.TestId == attempt.TestId && tr.StudentId == user.Id);

            if (testResult == null)
            {
                TempData["Error"] = "Test result not found!";
                return RedirectToAction("Dashboard");
            }

            // Get questions in the order they were presented
            List<TestQuestion> questions;
            if (!string.IsNullOrEmpty(attempt.ShuffledQuestionOrder))
            {
                var questionIds = attempt.ShuffledQuestionOrder.Split(',').Select(int.Parse).ToList();
                var allQuestions = await _context.TestQuestions
                    .Include(tq => tq.Question)
                        .ThenInclude(q => q!.Topic)
                    .Where(tq => tq.TestId == attempt.TestId)
                    .ToListAsync();
                
                questions = new List<TestQuestion>();
                foreach (var id in questionIds)
                {
                    var tq = allQuestions.FirstOrDefault(q => q.Id == id);
                    if (tq != null) questions.Add(tq);
                }
            }
            else
            {
                questions = await _context.TestQuestions
                    .Include(tq => tq.Question)
                        .ThenInclude(q => q!.Topic)
                    .Where(tq => tq.TestId == attempt.TestId)
                    .OrderBy(tq => tq.QuestionOrder)
                    .ToListAsync();
            }

            ViewBag.TestAttempt = attempt;
            ViewBag.TestResult = testResult;
            ViewBag.Questions = questions;
            ViewBag.ShowAnswers = attempt.Test!.ShowResultsImmediately;

            return View();
        }
    }
}

