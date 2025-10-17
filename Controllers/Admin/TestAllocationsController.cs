using CETExamApp.Data;
using CETExamApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class TestAllocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestAllocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? testId)
        {
            var allocationsQuery = _context.TestAllocations
                .Include(ta => ta.Test)
                .Include(ta => ta.Student)
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

            // Get students not already allocated to the selected test
            List<string> allocatedStudentIds = new List<string>();
            if (testId.HasValue)
            {
                allocatedStudentIds = await _context.TestAllocations
                    .Where(ta => ta.TestId == testId)
                    .Select(ta => ta.StudentId)
                    .ToListAsync();
            }

            var students = await _context.Users
                .Include(u => u.Group)
                .Where(u => u.GroupId != null && !allocatedStudentIds.Contains(u.Id))
                .ToListAsync();

            var studentRoles = new List<ApplicationUser>();
            foreach (var user in students)
            {
                var roles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
                var isStudent = roles.Any(r => r.RoleId == _context.Roles.FirstOrDefault(role => role.Name == "Student")?.Id);
                if (isStudent)
                    studentRoles.Add(user);
            }

            ViewBag.AllStudents = studentRoles;
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

            foreach (var studentId in studentIds)
            {
                // Check if already allocated
                var exists = await _context.TestAllocations
                    .AnyAsync(ta => ta.TestId == testId && ta.StudentId == studentId);
                
                if (!exists)
                {
                    var allocation = new TestAllocation
                    {
                        TestId = testId,
                        StudentId = studentId,
                        AllocatedDate = DateTime.UtcNow,
                        // Convert local time to UTC for storage
                        ScheduledStartTime = scheduledStartTime?.ToUniversalTime(),
                        ScheduledEndTime = scheduledEndTime?.ToUniversalTime(),
                        AllocatedBy = User.Identity?.Name
                    };

                    _context.TestAllocations.Add(allocation);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Test allocated to {studentIds.Count} student(s) successfully!";
            return RedirectToAction(nameof(Index), new { testId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateToGroup(int testId, int groupId, DateTime? scheduledStartTime, DateTime? scheduledEndTime)
        {
            // Get all students in the selected group
            var students = await _context.Users
                .Where(u => u.GroupId == groupId)
                .ToListAsync();

            if (!students.Any())
            {
                TempData["Error"] = "No students found in the selected group!";
                return RedirectToAction(nameof(Allocate), new { testId });
            }

            int allocatedCount = 0;
            foreach (var student in students)
            {
                // Check if already allocated
                var exists = await _context.TestAllocations
                    .AnyAsync(ta => ta.TestId == testId && ta.StudentId == student.Id);
                
                if (!exists)
                {
                    var allocation = new TestAllocation
                    {
                        TestId = testId,
                        StudentId = student.Id,
                        AllocatedDate = DateTime.UtcNow,
                        // Convert local time to UTC for storage
                        ScheduledStartTime = scheduledStartTime?.ToUniversalTime(),
                        ScheduledEndTime = scheduledEndTime?.ToUniversalTime(),
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

            allocation.ScheduledStartTime = scheduledStartTime.ToUniversalTime();
            allocation.ScheduledEndTime = scheduledEndTime.ToUniversalTime();

            _context.Update(allocation);
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

            foreach (var allocation in allocations)
            {
                allocation.ScheduledStartTime = scheduledStartTime;
                allocation.ScheduledEndTime = scheduledEndTime;
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
    }
}

