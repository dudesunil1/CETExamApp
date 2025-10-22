using CETExamApp.Data;
using CETExamApp.Models;
using CETExamApp.Models.ViewModels;
using CETExamApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestAllocationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TestAllocationsController> _logger;

        public TestAllocationsController(ApplicationDbContext context, ILogger<TestAllocationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? testId)
        {
            // Check if there are any test schedules
            var scheduleCount = await _context.TestSchedules.CountAsync();
            if (scheduleCount == 0)
            {
                TempData["Info"] = "No test schedules found. Create your first test allocation to see schedules here.";
            }

            var allocationsQuery = _context.TestAllocations
                .Include(ta => ta.Test)
                .Include(ta => ta.Student)
                .Include(ta => ta.TestSchedule)
                .AsQueryable();

            if (testId.HasValue)
                allocationsQuery = allocationsQuery.Where(ta => ta.TestId == testId.Value);

            var allocations = await allocationsQuery
                .OrderByDescending(ta => ta.AllocatedDate)
                .ToListAsync();

            ViewData["TestId"] = new SelectList(await _context.Tests.ToListAsync(), "Id", "Title", testId);
            return View(allocations);
        }

        public async Task<IActionResult> Allocate(int? testId)
        {
            var tests = await _context.Tests
                .Where(t => t.Status == TestStatus.Published || t.Status == TestStatus.Draft)
                .ToListAsync();
            
            ViewData["TestId"] = new SelectList(tests, "Id", "Title", testId);

            // Get groups
            var groups = await _context.Groups.Where(g => g.IsActive).ToListAsync();
            ViewData["Groups"] = new SelectList(groups, "Id", "Name");

            // Get all students organized by class
            var students = await _context.Users
                .Include(u => u.Group)
                .Include(u => u.Class)
                .Where(u => u.GroupId != null)
                .ToListAsync();

            var studentRoles = new List<ApplicationUser>();
            foreach (var user in students)
            {
                var roles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
                var isStudent = roles.Any(r => r.RoleId == _context.Roles.FirstOrDefault(role => role.Name == "Student")?.Id);
                if (isStudent)
                    studentRoles.Add(user);
            }

            // Group students by class for better organization
            var studentsByClass = studentRoles.GroupBy(s => s.Class?.Name ?? "No Class").ToList();
            ViewBag.StudentsByClass = studentsByClass;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateToStudents(int testId, List<string> studentIds, DateTime? scheduledStartTime, DateTime? scheduledEndTime)
        {
            if (studentIds == null || !studentIds.Any())
            {
                TempData["Error"] = "Please select at least one student!";
                return RedirectToAction(nameof(Allocate), new { testId });
            }

            if (!scheduledStartTime.HasValue || !scheduledEndTime.HasValue)
            {
                TempData["Error"] = "Please provide both start and end times for the test schedule!";
                return RedirectToAction(nameof(Allocate), new { testId });
            }

            // Create a new TestSchedule for this allocation
            // Store IST directly in database
            var testSchedule = new TestSchedule
            {
                TestId = testId,
                ScheduledStartTime = scheduledStartTime.Value,
                ScheduledEndTime = scheduledEndTime.Value,
                AllocatedBy = User.Identity?.Name,
                CreateDate = DateTimeExtensions.NowIST()
            };

            _context.TestSchedules.Add(testSchedule);
            await _context.SaveChangesAsync(); // Save to get the Schedule ID

            int allocatedCount = 0;
            foreach (var studentId in studentIds)
            {
                // Check if already allocated to this specific schedule (not just any schedule for this test)
                var exists = await _context.TestAllocations
                    .AnyAsync(ta => ta.ScheduleId == testSchedule.Id && ta.StudentId == studentId);
                
                if (!exists)
                {
                    var allocation = new TestAllocation
                    {
                        TestId = testId,
                        ScheduleId = testSchedule.Id,
                        StudentId = studentId,
                        AllocatedDate = DateTimeExtensions.NowIST(),
                        // Copy schedule times to allocation for easy access
                        ScheduledStartTime = testSchedule.ScheduledStartTime,
                        ScheduledEndTime = testSchedule.ScheduledEndTime,
                        AllocatedBy = User.Identity?.Name
                    };

                    _context.TestAllocations.Add(allocation);
                    allocatedCount++;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Test allocated to {allocatedCount} student(s) successfully!";
            return RedirectToAction(nameof(Index), new { testId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateToGroup(int testId, int groupId, DateTime? scheduledStartTime, DateTime? scheduledEndTime)
        {
            if (!scheduledStartTime.HasValue || !scheduledEndTime.HasValue)
            {
                TempData["Error"] = "Please provide both start and end times for the test schedule!";
                return RedirectToAction(nameof(Allocate), new { testId });
            }

            // Get all students in the selected group
            var students = await _context.Users
                .Where(u => u.GroupId == groupId)
                .ToListAsync();

            if (!students.Any())
            {
                TempData["Error"] = "No students found in the selected group!";
                return RedirectToAction(nameof(Allocate), new { testId });
            }

            // Create a new TestSchedule for this allocation
            // Store IST directly in database
            var testSchedule = new TestSchedule
            {
                TestId = testId,
                ScheduledStartTime = scheduledStartTime.Value,
                ScheduledEndTime = scheduledEndTime.Value,
                AllocatedBy = User.Identity?.Name,
                CreateDate = DateTimeExtensions.NowIST()
            };

            _context.TestSchedules.Add(testSchedule);
            await _context.SaveChangesAsync(); // Save to get the Schedule ID

            int allocatedCount = 0;
            foreach (var student in students)
            {
                // Check if already allocated to this specific schedule (not just any schedule for this test)
                var exists = await _context.TestAllocations
                    .AnyAsync(ta => ta.ScheduleId == testSchedule.Id && ta.StudentId == student.Id);
                
                if (!exists)
                {
                    var allocation = new TestAllocation
                    {
                        TestId = testId,
                        ScheduleId = testSchedule.Id,
                        StudentId = student.Id,
                        AllocatedDate = DateTimeExtensions.NowIST(),
                        // Copy schedule times to allocation for easy access
                        ScheduledStartTime = testSchedule.ScheduledStartTime,
                        ScheduledEndTime = testSchedule.ScheduledEndTime,
                        AllocatedBy = User.Identity?.Name
                    };

                    _context.TestAllocations.Add(allocation);
                    allocatedCount++;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Test allocated to {allocatedCount} student(s) in the group successfully!";
            return RedirectToAction(nameof(Index), new { testId });
        }

        public async Task<IActionResult> Reschedule(int? id)
        {
            if (id == null) return NotFound();

            var allocation = await _context.TestAllocations
                .Include(ta => ta.Test)
                .Include(ta => ta.Student)
                .FirstOrDefaultAsync(ta => ta.Id == id);

            if (allocation == null) return NotFound();

            return View(allocation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule(int id, DateTime scheduledStartTime, DateTime scheduledEndTime)
        {
            var allocation = await _context.TestAllocations.FindAsync(id);
            if (allocation == null) return NotFound();

            // Store IST directly in database
            allocation.ScheduledStartTime = scheduledStartTime;
            allocation.ScheduledEndTime = scheduledEndTime;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Test rescheduled successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RescheduleMultiple(int? testId)
        {
            if (testId == null) return NotFound();

            var test = await _context.Tests.FindAsync(testId);
            if (test == null) return NotFound();

            var allocations = await _context.TestAllocations
                .Include(ta => ta.Student)
                .Where(ta => ta.TestId == testId)
                .ToListAsync();

            ViewBag.Test = test;
            return View(allocations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RescheduleMultiple(int testId, DateTime scheduledStartTime, DateTime scheduledEndTime)
        {
            var allocations = await _context.TestAllocations
                .Where(ta => ta.TestId == testId)
                .ToListAsync();

            // Store IST directly in database
            var utcStartTime = scheduledStartTime;
            var utcEndTime = scheduledEndTime;

            foreach (var allocation in allocations)
            {
                allocation.ScheduledStartTime = utcStartTime;
                allocation.ScheduledEndTime = utcEndTime;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Test rescheduled for {allocations.Count} student(s) successfully!";
            return RedirectToAction(nameof(Index), new { testId });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var allocation = await _context.TestAllocations
                .Include(ta => ta.Test)
                .Include(ta => ta.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allocation == null) return NotFound();

            return View(allocation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allocation = await _context.TestAllocations.FindAsync(id);
            if (allocation != null)
            {
                _context.TestAllocations.Remove(allocation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Test allocation removed successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetTestDuration(int testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            if (test == null)
                return Json(new { error = "Test not found" });

            return Json(new { durationMinutes = test.DurationMinutes });
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsByGroup(int groupId)
        {
            try
            {
                var students = await _context.Users
                    .Include(u => u.Group)
                    .Include(u => u.Class)
                    .Where(u => u.GroupId == groupId && u.IsActive)
                    .ToListAsync();

                // Get the Student role ID
                var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
                if (studentRole == null)
                {
                    return Json(new List<object>());
                }

                // Filter for students only
                var studentRoles = new List<ApplicationUser>();
                foreach (var user in students)
                {
                    var roles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
                    var isStudent = roles.Any(r => r.RoleId == studentRole.Id);
                    if (isStudent)
                        studentRoles.Add(user);
                }

                var result = studentRoles.Select(s => new {
                    id = s.Id,
                    firstName = s.FirstName,
                    lastName = s.LastName,
                    studentUsername = s.StudentUsername,
                    className = s.Class?.Name ?? "No Class"
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting students for group {GroupId}", groupId);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> DebugDatabase()
        {
            try
            {
                var debugInfo = new
                {
                    totalTests = await _context.Tests.CountAsync(),
                    totalTestSchedules = await _context.TestSchedules.CountAsync(),
                    totalTestAllocations = await _context.TestAllocations.CountAsync(),
                    totalStudents = await _context.Users.CountAsync(),
                    totalGroups = await _context.Groups.CountAsync(),
                    testSchedules = await _context.TestSchedules
                        .Include(ts => ts.Test)
                        .Include(ts => ts.TestAllocations)
                        .Select(ts => new
                        {
                            id = ts.Id,
                            testId = ts.TestId,
                            testTitle = ts.Test.Title,
                            scheduledStartTime = ts.ScheduledStartTime,
                            scheduledEndTime = ts.ScheduledEndTime,
                            studentCount = ts.TestAllocations.Count,
                            allocatedBy = ts.AllocatedBy,
                            createDate = ts.CreateDate
                        })
                        .ToListAsync()
                };
                
                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in debug database");
                return Json(new { error = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult TestEndpoint()
        {
            return Json(new { message = "Test endpoint working", timestamp = DateTime.Now });
        }

        [HttpGet]
        public IActionResult SimpleTest()
        {
            return Json(new { message = "TestAllocationsController is working!", timestamp = DateTime.Now });
        }

        [HttpGet]
        public async Task<IActionResult> LoadSchedules(int? testId = null, DateTime? date = null)
        {
            try
            {
                // Debug: Check total TestSchedules count
                var totalSchedules = await _context.TestSchedules.CountAsync();
                _logger.LogInformation($"Total TestSchedules in database: {totalSchedules}");
                
                // Debug: Check total Tests count
                var totalTests = await _context.Tests.CountAsync();
                _logger.LogInformation($"Total Tests in database: {totalTests}");
                
                // Debug: Check total TestAllocations count
                var totalAllocations = await _context.TestAllocations.CountAsync();
                _logger.LogInformation($"Total TestAllocations in database: {totalAllocations}");

                var query = _context.TestSchedules
                    .Include(ts => ts.Test)
                        .ThenInclude(t => t.Class)
                    .Include(ts => ts.TestAllocations)
                    .AsQueryable();

                // Filter by test if specified
                if (testId.HasValue)
                {
                    query = query.Where(ts => ts.TestId == testId.Value);
                    _logger.LogInformation($"Filtering by testId: {testId.Value}");
                }

                // Filter by date if specified
                if (date.HasValue)
                {
                    var startOfDay = date.Value.Date;
                    var endOfDay = startOfDay.AddDays(1);
                    query = query.Where(ts => ts.ScheduledStartTime >= startOfDay && ts.ScheduledStartTime < endOfDay);
                    _logger.LogInformation($"Filtering by date: {startOfDay} to {endOfDay}");
                }

                var schedules = await query
                    .OrderByDescending(ts => ts.ScheduledStartTime)
                    .ToListAsync();

                _logger.LogInformation($"Found {schedules.Count} schedules after filtering");

                var result = schedules.Select(s => new
                {
                    id = s.Id,
                    testTitle = s.Test?.Title ?? "Unknown Test",
                    testClass = s.Test?.Class?.Name ?? "No Class",
                    scheduledStartTime = s.ScheduledStartTime,
                    scheduledEndTime = s.ScheduledEndTime,
                    studentCount = s.TestAllocations?.Count ?? 0,
                    isCompleted = s.IsCompleted,
                    allocatedBy = s.AllocatedBy,
                    createDate = s.CreateDate
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting test schedules: {Message}", ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewScheduleStudents(int scheduleId)
        {
            try
            {
                var schedule = await _context.TestSchedules
                    .Include(ts => ts.Test)
                    .Include(ts => ts.TestAllocations)
                        .ThenInclude(ta => ta.Student)
                            .ThenInclude(s => s.Class)
                    .Include(ts => ts.TestAllocations)
                        .ThenInclude(ta => ta.Student)
                            .ThenInclude(s => s.Group)
                    .FirstOrDefaultAsync(ts => ts.Id == scheduleId);

                if (schedule == null)
                {
                    _logger.LogWarning("TestSchedule with ID {ScheduleId} not found", scheduleId);
                    TempData["Error"] = $"Test schedule with ID {scheduleId} not found. Please check if the schedule exists.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Schedule = schedule;
                return View(schedule.TestAllocations.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading schedule students for schedule {ScheduleId}", scheduleId);
                TempData["Error"] = "An error occurred while loading the schedule students.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> RescheduleSchedule(int scheduleId)
        {
            try
            {
                var schedule = await _context.TestSchedules
                    .Include(ts => ts.Test)
                    .FirstOrDefaultAsync(ts => ts.Id == scheduleId);

                if (schedule == null)
                {
                    return NotFound();
                }

                return View(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading schedule for reschedule {ScheduleId}", scheduleId);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSchedule(int scheduleId, DateTime scheduledStartTime, DateTime scheduledEndTime)
        {
            try
            {
                var schedule = await _context.TestSchedules.FindAsync(scheduleId);
                if (schedule == null)
                {
                    return Json(new { success = false, message = "Schedule not found" });
                }

                // Store IST directly in database
                schedule.ScheduledStartTime = scheduledStartTime;
                schedule.ScheduledEndTime = scheduledEndTime;

                // Update all related allocations
                var allocations = await _context.TestAllocations
                    .Where(ta => ta.ScheduleId == scheduleId)
                    .ToListAsync();

                foreach (var allocation in allocations)
                {
                    allocation.ScheduledStartTime = schedule.ScheduledStartTime;
                    allocation.ScheduledEndTime = schedule.ScheduledEndTime;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Schedule updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating schedule {ScheduleId}", scheduleId);
                return Json(new { success = false, message = "Error updating schedule" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSchedule(int scheduleId)
        {
            try
            {
                var schedule = await _context.TestSchedules
                    .Include(ts => ts.TestAllocations)
                    .FirstOrDefaultAsync(ts => ts.Id == scheduleId);

                if (schedule == null)
                {
                    return Json(new { success = false, message = "Schedule not found" });
                }

                // Remove all related allocations
                _context.TestAllocations.RemoveRange(schedule.TestAllocations);

                // Remove the schedule
                _context.TestSchedules.Remove(schedule);

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Schedule deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting schedule {ScheduleId}", scheduleId);
                return Json(new { success = false, message = "Error deleting schedule" });
            }
        }
    }
}

